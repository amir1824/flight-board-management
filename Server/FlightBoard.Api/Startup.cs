using System.Diagnostics;
using FlightBoard.Api.Data;
using FlightBoard.Api.Hubs;
using FlightBoard.Api.Middleware;
using FlightBoard.Api.Dal.Interfaces;
using FlightBoard.Api.Dal.Ef;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace FlightBoard.Api;

public class Startup(IConfiguration configuration, IHostEnvironment env)
{
    private readonly IConfiguration _config = configuration;
    private readonly IHostEnvironment _env = env;

    public WebApplicationBuilder ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddCors(opt =>
        {
            var fe = _config["Frontend:BaseUrl"] ?? "http://localhost:5173";
            opt.AddPolicy("corsapp", p => p
                .WithOrigins(fe)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
        });

        builder.Services.AddDbContextPool<AppDbContext>(opt =>
            opt.UseSqlite(_config.GetConnectionString("Default") ?? "Data Source=app.db"));

        builder.Services.AddScoped<IFlightDal, FlightEfDal>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        builder.Services.AddControllers();
        builder.Services.AddSignalR();
        builder.Services.AddHealthChecks();

        if (_env.IsDevelopment())
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "FlightBoard API",
                    Version = "v1",
                    Description = "Real-time flight board with SignalR & EF Core (SQLite)"
                });
            });

            builder.Services.AddHttpLogging(o =>
            {
                o.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders |
                                  HttpLoggingFields.ResponsePropertiesAndHeaders |
                                  HttpLoggingFields.ResponseStatusCode;
            });
        }

        return builder;
    }

    public void Configure(WebApplication app)
    {
        var runMigrations =
            _env.IsProduction() ||
            _config.GetValue<bool>("Database:RunMigrationsOnStartup");

        if (runMigrations)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
            db.Database.ExecuteSqlRaw("PRAGMA journal_mode=WAL;");
            db.Database.ExecuteSqlRaw("PRAGMA foreign_keys=ON;");
        }

        if (_env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpLogging();

            // פתיחה אוטומטית של Swagger על Start (DEV בלבד)
            var openOnStart = _config.GetValue("Dev:OpenBrowserOnStart", true);
            if (openOnStart)
            {
                app.Lifetime.ApplicationStarted.Register(() =>
                {
                    var url = ResolveSwaggerUrl(app);
                    TryOpenBrowser(url);
                    Console.WriteLine($"Swagger UI available at {url}");
                });
            }
        }
        else
        {
            app.UseHttpsRedirection();
        }

        app.UseCors("corsapp");

        app.UseHttpExceptionMiddleware();

        
        app.MapHealthChecks("/health");
        app.MapHub<FlightsHub>("/hubs/flights");
        app.MapControllers();
    }


    private string ResolveSwaggerUrl(WebApplication app)
    {
        var urls = app.Urls?.ToArray() ?? Array.Empty<string>();
        var baseUrl =
            urls.FirstOrDefault(u => u.StartsWith("https://", StringComparison.OrdinalIgnoreCase)) ??
            urls.FirstOrDefault(u => u.StartsWith("http://", StringComparison.OrdinalIgnoreCase));

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            var envUrls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
            baseUrl = envUrls?.Split(';', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        }

        if (string.IsNullOrWhiteSpace(baseUrl))
            baseUrl = "http://localhost:5000";

        return $"{baseUrl.TrimEnd('/')}/swagger";
    }

    private static void TryOpenBrowser(string url)
    {
        try
        {
            if (OperatingSystem.IsWindows())
            {
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            }
            else if (OperatingSystem.IsMacOS())
            {
                Process.Start("open", url);
            }
            else if (OperatingSystem.IsLinux())
            {
                Process.Start("xdg-open", url);
            }
            else
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
        }
        catch {  }
    }
}
