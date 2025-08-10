using FlightBoard.Api;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);

var NestTheme = new AnsiConsoleTheme(new Dictionary<ConsoleThemeStyle, string>
{
    [ConsoleThemeStyle.Text] = "\x1b[37m",
    [ConsoleThemeStyle.SecondaryText] = "\x1b[90m",
    [ConsoleThemeStyle.TertiaryText] = "\x1b[90m",
    [ConsoleThemeStyle.Invalid] = "\x1b[33m",
    [ConsoleThemeStyle.Null] = "\x1b[90m",
    [ConsoleThemeStyle.Name] = "\x1b[37m",
    [ConsoleThemeStyle.String] = "\x1b[37m",
    [ConsoleThemeStyle.Number] = "\x1b[37m",
    [ConsoleThemeStyle.Boolean] = "\x1b[37m",
    [ConsoleThemeStyle.Scalar] = "\x1b[37m",
    [ConsoleThemeStyle.LevelVerbose] = "\x1b[90m",
    [ConsoleThemeStyle.LevelDebug] = "\x1b[36m",
    [ConsoleThemeStyle.LevelInformation] = "\x1b[32m",
    [ConsoleThemeStyle.LevelWarning] = "\x1b[33m",
    [ConsoleThemeStyle.LevelError] = "\x1b[31m",
    [ConsoleThemeStyle.LevelFatal] = "\x1b[31m"
});

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Query", LogEventLevel.Error)
    .Enrich.FromLogContext()                          
    .Enrich.WithProperty("Application", "FlightBoard") 
    .WriteTo.Console(
        theme: NestTheme,
        outputTemplate:
            "[{Timestamp:HH:mm:ss} {Level:u3}] [{Operation}] [{Entity}] Status:{StatusCode} {Message:lj}{NewLine}{Exception}",
        applyThemeToRedirectedOutput: true
    )
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Host.UseSerilog();

var startup = new Startup(builder.Configuration, builder.Environment);
var app = startup.ConfigureServices(builder).Build();
startup.Configure(app);

app.Run();