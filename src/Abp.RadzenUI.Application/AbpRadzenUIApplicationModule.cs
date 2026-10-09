using Abp.RadzenUI.ObjectExtending;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;
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

	public override void OnApplicationInitialization(ApplicationInitializationContext context)
	{
		// Module-level user extensions copy validation attributes (e.g. [Required]) onto
		// UpdateProfileDto. A ReadOnly / Hidden property cannot be filled on the profile page,
		// so those rules must not block saving the profile. Runs after every module has
		// registered its extension properties and after AbpRadzenUIProfileOptions is configured.
		var options = context.ServiceProvider.GetRequiredService<IOptions<AbpRadzenUIProfileOptions>>().Value;
		ProfileExtensionPropertyHelper.RelaxValidationForNonEditableProperties(
			ObjectExtensionManager.Instance.GetProperties<UpdateProfileDto>(),
			options
		);
	}
}