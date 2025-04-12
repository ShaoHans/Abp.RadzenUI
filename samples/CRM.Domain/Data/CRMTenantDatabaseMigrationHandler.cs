using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace CRM.Data;

public class CRMTenantDatabaseMigrationHandler(
    IEnumerable<ICRMDbSchemaMigrator> dbSchemaMigrators,
    ICurrentTenant currentTenant,
    IUnitOfWorkManager unitOfWorkManager,
    IDataSeeder dataSeeder,
    ITenantStore tenantStore,
    ILogger<CRMTenantDatabaseMigrationHandler> logger
)
    : IDistributedEventHandler<TenantCreatedEto>,
        IDistributedEventHandler<TenantConnectionStringUpdatedEto>,
        IDistributedEventHandler<ApplyDatabaseMigrationsEto>,
        ITransientDependency
{
    public async Task HandleEventAsync(TenantCreatedEto eventData)
    {
        await MigrateAndSeedForTenantAsync(
            eventData.Id,
            eventData.Properties.GetOrDefault("AdminEmail") ?? CRMConsts.AdminEmailDefaultValue,
            eventData.Properties.GetOrDefault("AdminPassword")
                ?? CRMConsts.AdminPasswordDefaultValue
        );
    }

    public async Task HandleEventAsync(TenantConnectionStringUpdatedEto eventData)
    {
        if (
            eventData.ConnectionStringName != ConnectionStrings.DefaultConnectionStringName
            || eventData.NewValue.IsNullOrWhiteSpace()
        )
        {
            return;
        }

        await MigrateAndSeedForTenantAsync(
            eventData.Id,
            CRMConsts.AdminEmailDefaultValue,
            CRMConsts.AdminPasswordDefaultValue
        );

        /* You may want to move your data from the old database to the new database!
         * It is up to you. If you don't make it, new database will be empty
         * (and tenant's admin password is reset to 1q2w3E*).
         */
    }

    public async Task HandleEventAsync(ApplyDatabaseMigrationsEto eventData)
    {
        if (eventData.TenantId == null)
        {
            return;
        }

        await MigrateAndSeedForTenantAsync(
            eventData.TenantId.Value,
            CRMConsts.AdminEmailDefaultValue,
            CRMConsts.AdminPasswordDefaultValue
        );
    }

    private async Task MigrateAndSeedForTenantAsync(
        Guid tenantId,
        string adminEmail,
        string adminPassword
    )
    {
        try
        {
            using (currentTenant.Change(tenantId))
            {
                // Create database tables if needed
                using (
                    var uow = unitOfWorkManager.Begin(requiresNew: false, isTransactional: false)
                )
                {
                    var tenantConfiguration = await tenantStore.FindAsync(tenantId);
                    if (
                        tenantConfiguration?.ConnectionStrings != null
                        && !tenantConfiguration.ConnectionStrings.Default.IsNullOrWhiteSpace()
                    )
                    {
                        foreach (var migrator in dbSchemaMigrators)
                        {
                            await migrator.MigrateAsync();
                        }
                    }

                    await uow.CompleteAsync();
                }

                // Seed data
                using (var uow = unitOfWorkManager.Begin(requiresNew: false, isTransactional: true))
                {
                    await dataSeeder.SeedAsync(
                        new DataSeedContext(tenantId)
                            .WithProperty(
                                IdentityDataSeedContributor.AdminEmailPropertyName,
                                adminEmail
                            )
                            .WithProperty(
                                IdentityDataSeedContributor.AdminPasswordPropertyName,
                                adminPassword
                            )
                    );

                    await uow.CompleteAsync();
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogException(ex);
        }
    }
}
