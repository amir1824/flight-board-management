using System.Diagnostics;
using FlightBoard.Api.Data;
using FlightBoard.Api.Hubs;
using FlightBoard.Api.Middleware;
using FlightBoard.Api.Dal.Interfaces;
using FlightBoard.Api.Dal.Ef;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using FlightBoard.Api.Services.Logging.Interfaces;
using FlightBoard.Api.Services.Logging.Implementations;
using Serilog;
using FluentValidation;
using FluentValidation.AspNetCore;
using FlightBoard.Api.Features.Flights.Validation;

namespace FlightBoard.Api;

public class Startup(IConfiguration configuration, IHostEnvironment env)
{
    private readonly IConfiguration _config = configuration;
    private readonly IHostEnvironment _env = env;


    public WebApplicationBuilder ConfigureServices(WebApplicationBuilder builder)
    {
        ConfigureCors(builder);
        ConfigureDatabase(builder);
        ConfigureAppServices(builder);
        ConfigureFluentValidation(builder);

        if (_env.IsDevelopment())
            ConfigureSwagger(builder);

        return builder;
    }


    public void Configure(WebApplication app)
    {
        InitDatabase(app);

        if (_env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            ConfigureSerilogRequestLogging(app);
            AutoOpenSwagger(app);
        }
        else
        {
            app.UseHttpsRedirection();
        }

        app.UseCors("corsapp");
        app.UseRequestLogContext(); 
        app.UseMiddleware<HttpExceptionMiddleware>();
        app.MapHealthChecks("/health");
        app.MapHub<FlightsHub>("/hubs/flights");
        app.MapControllers();
    }


    private void ConfigureCors(WebApplicationBuilder builder)
    {
        var fe = _config["Frontend:BaseUrl"] ?? "http://localhost:5173";
        builder.Services.AddCors(opt =>
        {
            opt.AddPolicy("corsapp", p => p
                .WithOrigins(fe)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
        });
    }

    private void ConfigureDatabase(WebApplicationBuilder builder)
    {
        builder.Services.AddDbContextPool<AppDbContext>(opt =>
            opt.UseSqlite(_config.GetConnectionString("Default") ?? "Data Source=app.db"));
    }

    private void ConfigureAppServices(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IFlightDal, FlightEfDal>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<ILogContextAccessor, LogContextAccessor>();
        builder.Services.AddScoped<IAppLogger, AppLogger>();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddControllers();
        builder.Services.AddSignalR();
        builder.Services.AddHealthChecks();
        builder.Services.AddHostedService<FlightStatusNotifier>();
    }

    private static void ConfigureFluentValidation(WebApplicationBuilder builder)
{
    // כדי שלא נקבל 400 אוטומטי על ModelState וננהל הכל ידנית בבייס
    builder.Services.AddControllers()
        .ConfigureApiBehaviorOptions(o => o.SuppressModelStateInvalidFilter = true);

    // טוען את כל הוולידטורים (FlightValidator וכו')
    builder.Services.AddValidatorsFromAssemblyContaining<FlightValidator>();

    // שים לב: לא קוראים ל-AddFluentValidationAutoValidation()
}

    private void ConfigureSwagger(WebApplicationBuilder builder)
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
    }

    private void ConfigureSerilogRequestLogging(WebApplication app)
    {
        app.UseSerilogRequestLogging(opts =>
        {
            opts.MessageTemplate =
                "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

            opts.EnrichDiagnosticContext = (ctx, http) =>
            {
                ctx.Set("Operation", "HTTP");
                ctx.Set("Entity", $"{http.Request.Method} {http.Request.Path}");
                ctx.Set("StatusCode", http.Response.StatusCode);
            };
        });
    }

    private void InitDatabase(WebApplication app)
    {
        var runMigrations = _env.IsProduction() || _config.GetValue<bool>("Database:RunMigrationsOnStartup");

        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (runMigrations)
            db.Database.Migrate();
        else if (_env.IsDevelopment())
            db.Database.EnsureCreated();

        db.Database.ExecuteSqlRaw("PRAGMA journal_mode=WAL;");
        db.Database.ExecuteSqlRaw("PRAGMA foreign_keys=ON;");
    }

    private void AutoOpenSwagger(WebApplication app)
    {
        if (!_config.GetValue("Dev:OpenBrowserOnStart", true)) return;

        app.Lifetime.ApplicationStarted.Register(() =>
        {
            var url = ResolveSwaggerUrl(app);
            TryOpenBrowser(url);
            Console.WriteLine($"Swagger UI available at {url}");
        });
    }

    private string ResolveSwaggerUrl(WebApplication app)
    {
        var urls = app.Urls?.ToArray() ?? Array.Empty<string>();
        var baseUrl =
            urls.FirstOrDefault(u => u.StartsWith("https://", StringComparison.OrdinalIgnoreCase)) ??
            urls.FirstOrDefault(u => u.StartsWith("http://", StringComparison.OrdinalIgnoreCase)) ??
            Environment.GetEnvironmentVariable("ASPNETCORE_URLS")?.Split(';', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ??
            "http://localhost:5000";

        return $"{baseUrl.TrimEnd('/')}/swagger";
    }

    private static void TryOpenBrowser(string url)
    {
        try
        {
            if (OperatingSystem.IsWindows())
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            else if (OperatingSystem.IsMacOS())
                Process.Start("open", url);
            else if (OperatingSystem.IsLinux())
                Process.Start("xdg-open", url);
            else
                Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
        }
        catch
        {
            // ignore
        }
    }
}
