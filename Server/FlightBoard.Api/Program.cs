using FlightBoard.Api;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) =>
{
    cfg.Enrich.FromLogContext()
       .MinimumLevel.Debug()
       .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
       .WriteTo.Console(
           outputTemplate: "\u001b[37m[{Timestamp:HH:mm:ss} {Level:u3}]\u001b[0m {Message:lj}{NewLine}{Exception}",
           theme: AnsiConsoleTheme.Code
       );
});

var startup = new Startup(builder.Configuration, builder.Environment);
var app = startup.ConfigureServices(builder).Build();
startup.Configure(app);

app.Run();
