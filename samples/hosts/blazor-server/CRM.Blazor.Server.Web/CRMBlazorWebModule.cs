using Abp.RadzenUI;
using Abp.RadzenUI.Localization;
using CRM.Blazor.Server.Web.Components.Pages;
using CRM.Blazor.Server.Web.Menus;
using CRM.EntityFrameworkCore;
using CRM.Localization;
using CRM.MultiTenancy;
using Microsoft.AspNetCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.OpenApi.Models;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Validation.AspNetCore;
using Volo.Abp;
using Volo.Abp.AspNetCore.Authentication.JwtBearer;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Auditing;
using Volo.Abp.AutoMapper;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.OpenIddict;
using Volo.Abp.Security.Claims;
using Volo.Abp.Swashbuckle;
using Volo.Abp.Timing;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;

namespace CRM.Blazor;

[DependsOn(
    typeof(CRMApplicationModule),
    typeof(CRMEntityFrameworkCoreModule),
    typeof(CRMHttpApiModule),
    typeof(AbpRadzenUIModule),
    typeof(AbpAspNetCoreMultiTenancyModule),
    typeof(AbpAspNetCoreAuthenticationJwtBearerModule),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpSwashbuckleModule)
)]
public class CRMBlazorWebModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(
                typeof(CRMResource),
                typeof(CRMDomainModule).Assembly,
                typeof(CRMDomainSharedModule).Assembly,
                typeof(CRMApplicationModule).Assembly,
                typeof(CRMApplicationContractsModule).Assembly,
                typeof(CRMBlazorWebModule).Assembly
            );
        });

        PreConfigure<OpenIddictBuilder>(builder =>
        {
            builder.AddValidation(options =>
            {
                options.AddAudiences("CRM");
                options.UseLocalServer();
                options.UseAspNetCore();
            });
        });

        if (!hostingEnvironment.IsDevelopment())
        {
            PreConfigure<AbpOpenIddictAspNetCoreOptions>(options =>
            {
                options.AddDevelopmentEncryptionAndSigningCertificate = false;
            });

            PreConfigure<OpenIddictServerBuilder>(serverBuilder =>
            {
                serverBuilder.AddProductionEncryptionAndSigningCertificate(
                    "openiddict.pfx",
                    "9b26e3c2-ae0a-4c47-bb89-606fcd97cf0b"
                );
                serverBuilder.SetIssuer(new Uri(configuration["AuthServer:Authority"]!));
            });
        }
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        if (!configuration.GetValue<bool>("App:DisablePII"))
        {
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
        }

        if (!configuration.GetValue<bool>("AuthServer:RequireHttpsMetadata"))
        {
            Configure<OpenIddictServerAspNetCoreOptions>(options =>
            {
                options.DisableTransportSecurityRequirement = true;
            });
        }

        Configure<AbpAntiForgeryOptions>(options =>
        {
            options.AutoValidate = false;
        });

        ConfigureAbpRadzenUI();
        ConfigureAuthentication(context);

        // configure external login
        ConfigureOidcAuthentication(context, configuration);

        ConfigureAutoMapper();
        ConfigureVirtualFileSystem(hostingEnvironment);
        ConfigureSwaggerServices(context.Services);
        ConfigureAutoApiControllers();

        Configure<AbpClockOptions>(options =>
        {
            options.Kind = DateTimeKind.Utc;
        });

        Configure<AbpAuditingOptions>(options =>
        {
            options.EntityHistorySelectors.AddAllEntities();
        });
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context)
    {
        context.Services.ForwardIdentityAuthenticationForBearer(
            OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme
        );
        context.Services.Configure<AbpClaimsPrincipalFactoryOptions>(options =>
        {
            options.IsDynamicClaimsEnabled = true;
        });
    }

    private void ConfigureOidcAuthentication(
        ServiceConfigurationContext context,
        IConfiguration configuration
    )
    {
        if (configuration.GetSection("AzureAd").Exists())
        {
            context
                .Services.AddAuthentication()
                .AddOpenIdConnect(
                    "AzureOpenId",
                    "Azure Active Directory",
                    options =>
                    {
                        options.Authority =
                            $"{configuration["AzureAd:Instance"]}{configuration["AzureAd:TenantId"]}/v2.0/";
                        options.ClientId = configuration["AzureAd:ClientId"];
                        options.ClientSecret = configuration["AzureAd:ClientSecret"];
                        options.ResponseType = OpenIdConnectResponseType.Code;
                        options.CallbackPath = configuration["AzureAd:CallbackPath"];
                        options.RequireHttpsMetadata = false;
                        options.SaveTokens = true;
                        options.GetClaimsFromUserInfoEndpoint = true;
                        options.SignInScheme = IdentityConstants.ExternalScheme;

                        options.Scope.Add("email");
                    }
                );
        }
    }

    private void ConfigureVirtualFileSystem(IWebHostEnvironment hostingEnvironment)
    {
        if (hostingEnvironment.IsDevelopment())
        {
            var contentRootPath = Path.GetFullPath(Path.Combine(hostingEnvironment.ContentRootPath, "..", "..")); ;
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<CRMDomainSharedModule>(
                    Path.Combine(
                        contentRootPath,
                        $"..{Path.DirectorySeparatorChar}CRM.Domain.Shared"
                    )
                );
                options.FileSets.ReplaceEmbeddedByPhysical<CRMDomainModule>(
                    Path.Combine(
                        contentRootPath,
                        $"..{Path.DirectorySeparatorChar}CRM.Domain"
                    )
                );
                options.FileSets.ReplaceEmbeddedByPhysical<CRMApplicationContractsModule>(
                    Path.Combine(
                        contentRootPath   ,
                        $"..{Path.DirectorySeparatorChar}CRM.Application.Contracts"
                    )
                );
                options.FileSets.ReplaceEmbeddedByPhysical<CRMApplicationModule>(
                    Path.Combine(
                        contentRootPath,
                        $"..{Path.DirectorySeparatorChar}CRM.Application"
                    )
                );
                options.FileSets.ReplaceEmbeddedByPhysical<CRMBlazorWebModule>(
                    hostingEnvironment.ContentRootPath
                );
            });
        }
    }

    private void ConfigureSwaggerServices(IServiceCollection services)
    {
        services.AddAbpSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "CRM API", Version = "v1" });
            options.DocInclusionPredicate((docName, description) => true);
            options.CustomSchemaIds(type => type.FullName);
        });
    }

    private void ConfigureAutoApiControllers()
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(CRMApplicationModule).Assembly);
        });
    }

    private void ConfigureAutoMapper()
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<CRMBlazorWebModule>();
        });
    }

    private void ConfigureAbpRadzenUI()
    {
        // Configure AbpRadzenUI
        Configure<AbpRadzenUIOptions>(options =>
        {
            // this is very imporant to set current web application's pages to the AbpRadzenUI module
            options.RouterAdditionalAssemblies = [typeof(Home).Assembly];

            // other settings
            //options.TitleBar = new TitleBarSettings
            //{
            //    ShowLanguageMenu = false,
            //    Title = "CRM"
            //};
            //options.LoginPage = new LoginPageSettings
            //{
            //    LogoPath = "xxx/xx.png"
            //};
            options.Theme = new ThemeSettings
            {
                EnablePremiumTheme = true,
            };

            // configure external login provider icon
            options.ExternalLogin.Providers.Add(new ExternalLoginProvider("AzureOpenId", "images/microsoft-logo.svg"));
        });

        // Configure AbpMultiTenancyOptions, this will affect login page that whether need to switch tenants
        Configure<AbpMultiTenancyOptions>(options =>
        {
            options.IsEnabled = MultiTenancyConsts.IsEnabled;
        });

        // Configure AbpLocalizationOptions
        Configure<AbpLocalizationOptions>(options =>
        {
            // set AbpRadzenUIResource as BaseTypes for your application's localization resources
            var crmResource = options.Resources.Get<CRMResource>();
            crmResource.AddBaseTypes(typeof(AbpRadzenUIResource));

            // if you don't want to use the default language list, you can clear it and add your own languages
            options.Languages.Clear();
            options.Languages.Add(new LanguageInfo("en", "en", "English"));
            options.Languages.Add(new LanguageInfo("fr", "fr", "Français"));
            options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
        });

        // Configure your web application's navigation menu
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new CRMMenuContributor());
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var env = context.GetEnvironment();
        var app = context.GetApplicationBuilder();

        app.UseAbpRequestLocalization();

        if (!env.IsDevelopment())
        {
            app.UseErrorPage();
            app.UseHsts();
        }

        app.UseCorrelationId();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAntiforgery();
        app.UseAbpSecurityHeaders();
        app.UseAuthentication();
        app.UseAbpOpenIddictValidation();

        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }

        app.UseUnitOfWork();
        app.UseDynamicClaims();
        app.UseAuthorization();
        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "CRM API");
        });
        app.UseAuditing();
        app.UseAbpSerilogEnrichers();

        // use RadzenUI
        app.UseRadzenUI();
    }
}
