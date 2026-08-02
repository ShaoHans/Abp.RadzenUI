using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Caching;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement;

namespace Abp.RadzenUI.LinkAccounts;

[DependsOn(typeof(AbpCachingModule), typeof(AbpIdentityDomainModule), typeof(AbpTenantManagementDomainModule))]
public class AbpRadzenUILinkAccountsModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpRadzenUILinkAccountsOptions>(options =>
        {
            options.ConfigureDefaults();
        });

        context.Services.AddOptions<AbpRadzenUILinkAccountsOptions>();
        context.Services.AddTransient<ILinkedAccountFlowStateStore, LinkedAccountFlowStateStore>();
        context.Services.AddTransient<ILinkedAccountAppService, LinkedAccountAppService>();
    }
}