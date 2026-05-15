using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Account;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;

namespace Abp.RadzenUI;

[DependsOn(
	typeof(AbpRadzenUIDomainModule),
	typeof(AbpRadzenUIApplicationContractsModule),
	typeof(AbpPermissionManagementApplicationModule),
	typeof(AbpFeatureManagementApplicationModule),
	typeof(AbpIdentityApplicationModule),
	typeof(AbpAccountApplicationModule),
	typeof(AbpTenantManagementApplicationModule),
	typeof(AbpSettingManagementApplicationModule)
)]
public class AbpRadzenUIApplicationModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		context.Services.AddMapperlyObjectMapper();
	}
}