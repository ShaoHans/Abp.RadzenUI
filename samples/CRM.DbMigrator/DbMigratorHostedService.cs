using System.Threading;
using System.Threading.Tasks;
using CRM.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Volo.Abp;
using Volo.Abp.Data;

namespace CRM.DbMigrator;

public class DbMigratorHostedService(
    IHostApplicationLifetime hostApplicationLifetime,
    IConfiguration configuration
) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var application = await AbpApplicationFactory.CreateAsync<CRMDbMigratorModule>(
            options =>
            {
                options.Services.ReplaceConfiguration(configuration);
                options.UseAutofac();
                options.Services.AddLogging(c => c.AddSerilog());
                options.AddDataMigrationEnvironment();
            }
        );
        await application.InitializeAsync();

        await application
            .ServiceProvider.GetRequiredService<CRMDbMigrationService>()
            .MigrateAsync();

        await application.ShutdownAsync();

        hostApplicationLifetime.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
