using Abp.RadzenUI.Localization;
using Volo.Abp.AuditLogging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using Volo.Abp.VirtualFileSystem;

namespace Abp.RadzenUI;

[DependsOn(
	typeof(AbpBackgroundJobsDomainSharedModule),
	typeof(AbpAuditLoggingDomainSharedModule),
	typeof(AbpFeatureManagementDomainSharedModule),
	typeof(AbpPermissionManagementDomainSharedModule),
	typeof(AbpSettingManagementDomainSharedModule),
	typeof(AbpIdentityDomainSharedModule),
	typeof(AbpTenantManagementDomainSharedModule)
)]
public class AbpRadzenUIDomainSharedModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		Configure<AbpVirtualFileSystemOptions>(options =>
		{
			options.FileSets.AddEmbedded<AbpRadzenUIDomainSharedModule>("Abp.RadzenUI");
		});

		Configure<AbpLocalizationOptions>(options =>
		{
			options.Resources.Add<AbpRadzenUIResource>("en").AddVirtualJson("/Localization/UI");
		});

		Configure<AbpExceptionLocalizationOptions>(options =>
		{
			options.MapCodeNamespace("AbpRadzenUI", typeof(AbpRadzenUIResource));
		});
	}
}