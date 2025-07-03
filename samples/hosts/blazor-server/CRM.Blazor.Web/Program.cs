using Serilog;
using Serilog.Events;

namespace CRM.Blazor;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            Log.Information("Starting web host.");
            var builder = WebApplication.CreateBuilder(args);
            builder
                .Host.AddAppSettingsSecretsJson()
                .UseAutofac()
                .UseSerilog(
                    (context, services, loggerConfiguration) =>
                    {
                        loggerConfiguration
#if DEBUG
                            .MinimumLevel.Debug()
#else
                            .MinimumLevel.Information()
#endif
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                            .MinimumLevel.Override(
                                "Microsoft.EntityFrameworkCore",
                                LogEventLevel.Warning
                            )
                            .Enrich.FromLogContext()
                            .WriteTo.Async(c => c.File("Logs/logs.txt", rollingInterval: RollingInterval.Day))
                            .WriteTo.Async(c => c.Console());
                    }
                );
            await builder.AddApplicationAsync<CRMBlazorWebModule>();
            var app = builder.Build();
            await app.InitializeApplicationAsync();
            await app.RunAsync();
            return 0;
        }
        catch (Exception ex)
        {
            if (ex is HostAbortedException)
            {
                throw;
            }

            Log.Fatal(ex, "Host terminated unexpectedly!");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
