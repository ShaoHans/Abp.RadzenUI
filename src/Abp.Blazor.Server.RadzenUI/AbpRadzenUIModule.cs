using Abp.RadzenUI.Blazor.SettingManagement;
using Abp.RadzenUI.Bundling;
using Abp.RadzenUI.Localization;
using Abp.RadzenUI.Menus;
using Abp.RadzenUI.Services;
using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Radzen;
using Volo.Abp.AspNetCore.Components.Messages;
using Volo.Abp.AspNetCore.Components.Server.Configuration;
using Volo.Abp.AspNetCore.Components.Web;
using Volo.Abp.AspNetCore.Components.Web.Configuration;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.AuditLogging.Localization;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.ExceptionHandling.Localization;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.Localization;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Volo.Abp.UI.Navigation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace Abp.RadzenUI;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpAuditLoggingEntityFrameworkCoreModule),
    typeof(AbpPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpSettingManagementEntityFrameworkCoreModule),
    typeof(AbpTenantManagementEntityFrameworkCoreModule),
    typeof(AbpIdentityEntityFrameworkCoreModule),
    typeof(AbpIdentityAspNetCoreModule),
    typeof(AbpAspNetCoreComponentsWebModule)
)]
public class AbpRadzenUIModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AbpRadzenUIModule>("Abp.RadzenUI");
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options
                .Resources.Add<AbpRadzenUIResource>("en")
                .AddBaseTypes(
                    typeof(AbpValidationResource),
                    typeof(AbpUiResource),
                    typeof(AbpExceptionHandlingResource),
                    typeof(AuditLoggingResource),
                    typeof(AbpSettingManagementResource)
                )
                .AddVirtualJson("/Localization/UI");
        });

        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<AbpRadzenUIModule>();
        });

        // Add services to the container.
        context.Services.AddRazorComponents().AddInteractiveServerComponents();

        // Add Radzen.Blazor services
        context.Services.AddRadzenComponents();
        context.Services.AddRadzenQueryStringThemeService();
        context.Services.AddCascadingAuthenticationState();

        context.Services.Replace(
            ServiceDescriptor.Singleton<
                ICurrentApplicationConfigurationCacheResetService,
                BlazorServerCurrentApplicationConfigurationCacheResetService
            >()
        );

        // Replace the default IUiMessageService with Radzen.Blazor's implementation by NotificationService
        context.Services.Replace(
            ServiceDescriptor.Transient<IUiMessageService, RadzenUiMessageService>()
        );

        Configure<AbpBundlingOptions>(options =>
        {
            options.StyleBundles.Add(
                BlazorRadzenThemeBundles.Styles.Global,
                bundle =>
                {
                    bundle.AddContributors(typeof(BlazorGlobalStyleContributor));
                }
            );

            options.ScriptBundles.Add(
                BlazorRadzenThemeBundles.Styles.Global,
                bundle =>
                {
                    bundle.AddContributors(typeof(BlazorGlobalScriptContributor));
                }
            );
        });

        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Clear();
            options.MenuContributors.Add(new DefaultRadzenMenuContributor());
            options.MenuContributors.Add(new AbpIdentityMenuContributor());
            options.MenuContributors.Add(new AbpTenantMenuContributor());
            options.MenuContributors.Add(new AuditLoggingMenuContributor());
            options.MenuContributors.Add(new SettingManagementMenuContributor());
        });

        Configure<SettingManagementComponentOptions>(options =>
        {
            options.Contributors.Add(new EmailingPageContributor());
            options.Contributors.Add(new TimeZonePageContributor());
        });
    }
}
