using System;
using System.Threading.Tasks;
using CRM.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;

namespace CRM.EntityFrameworkCore;

public class EntityFrameworkCoreCRMDbSchemaMigrator(IServiceProvider serviceProvider)
    : ICRMDbSchemaMigrator,
        ITransientDependency
{
    public async Task MigrateAsync()
    {
        /* We intentionally resolving the CRMDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await serviceProvider.GetRequiredService<CRMDbContext>().Database.MigrateAsync();
    }
}
