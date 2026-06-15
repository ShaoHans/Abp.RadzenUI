using Abp.RadzenUI.Features.Settings;
using Abp.RadzenUI.Infrastructure.Bundling;
using Abp.RadzenUI.Features.Avatar;
using Abp.RadzenUI.EntityFrameworkCore;
using Abp.RadzenUI.LinkAccounts;
using Abp.RadzenUI.Localization;
using Abp.RadzenUI.Menus;
using Abp.RadzenUI.Infrastructure.Navigation;
using Abp.RadzenUI.Infrastructure.Services;
using Localization.Resources.AbpUi;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Radzen;
using Volo.Abp.AspNetCore.Components.Messages;
using Volo.Abp.AspNetCore.Components.Notifications;
using Volo.Abp.AspNetCore.Components.Server.Configuration;
using Volo.Abp.AspNetCore.Components.Web;
using Volo.Abp.AspNetCore.Components.Web.Configuration;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Auditing;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.AuditLogging.Localization;
using Volo.Abp.Autofac;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.Localization;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Volo.Abp.UI.Navigation;

namespace Abp.RadzenUI;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpRadzenUIApplicationModule),
    typeof(AbpRadzenUIEntityFrameworkCoreModule),
    typeof(AbpRadzenUILinkAccountsModule),
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
        AvatarModuleExtensionConfigurator.Configure();

        //Configure<AbpAspNetCoreAuditingOptions>(options =>
        //{
        //    options.IgnoredUrls.AddIfNotContains("/_blazor");
        //});

        var configuration = context.Services.GetConfiguration();

        Configure<AbpLocalizationOptions>(options =>
        {
            options
                .Resources.Get<AbpRadzenUIResource>()
                .AddBaseTypes(
                    typeof(AbpUiResource),
                    typeof(AuditLoggingResource),
                    typeof(AbpSettingManagementResource)
                )
                ;
        });

        Configure<CookieAuthenticationOptions>(
            IdentityConstants.ApplicationScheme,
            options =>
            {
                options.Events ??= new CookieAuthenticationEvents();

                options.Events.OnRedirectToAccessDenied = ctx =>
                {
                    //ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                    ctx.Response.Redirect("/forbidden");
                    return Task.CompletedTask;
                };

                options.Events.OnRedirectToLogin = ctx =>
                {
                    //ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    ctx.Response.Redirect("/account/login");
                    return Task.CompletedTask;
                };
            }
        );

        context.Services.AddMapperlyObjectMapper();

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
        context.Services.Replace(
            ServiceDescriptor.Transient<IUiNotificationService, RadzenUiNotificationService>()
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
            options.MenuContributors.Add(new IdentitySecurityLogMenuContributor());
            options.MenuContributors.Add(new DataDictionaryMenuContributor());
            options.MenuContributors.Add(new MessageMenuContributor());
            options.MenuContributors.Add(new SettingManagementMenuContributor());
        });

        Configure<SettingManagementComponentOptions>(options =>
        {
            options.Contributors.Add(new EmailingPageContributor());
            options.Contributors.Add(new TimeZonePageContributor());
            options.Contributors.Add(new AccountPageContributor());
        });

        context.Services.AddSingleton(typeof(AbpBlazorMessageLocalizerHelper<>));
        context.Services.AddScoped<GridPageSizePreferenceService>();
        context.Services.AddScoped<MessageCenterState>();
        context.Services.AddScoped<MenuItemDecorationState>();
        context.Services.AddScoped(typeof(SideDialogState<>));
        context.Services.AddScoped<ISideDialogCoordinatorFactory, SideDialogCoordinatorFactory>();
        context.Services.AddTransient<IUploadService, DefaultUploadService>();
        context.Services.AddTransient<LinkedAccountSignInManager>();
    }
}
