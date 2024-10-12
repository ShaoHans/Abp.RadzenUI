using Abp.RadzenUI;
using Abp.RadzenUI.Components;
using CRM.Blazor.Web.Components.Pages;
using CRM.Blazor.Web.Menus;
using CRM.EntityFrameworkCore;
using CRM.Localization;
using CRM.MultiTenancy;
using Microsoft.AspNetCore.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Validation.AspNetCore;
using Volo.Abp;
using Volo.Abp.AspNetCore.Authentication.JwtBearer;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.AutoMapper;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.EntityFrameworkCore.PostgreSql;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;
using Volo.Abp.Swashbuckle;
using Volo.Abp.UI.Navigation;
using Volo.Abp.UI.Navigation.Urls;
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
    typeof(AbpEntityFrameworkCorePostgreSqlModule),
    typeof(AbpAuditLoggingEntityFrameworkCoreModule),
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

        //if (!hostingEnvironment.IsDevelopment())
        //{
        //    PreConfigure<AbpOpenIddictAspNetCoreOptions>(options =>
        //    {
        //        options.AddDevelopmentEncryptionAndSigningCertificate = false;
        //    });

        //    PreConfigure<OpenIddictServerBuilder>(serverBuilder =>
        //    {
        //        serverBuilder.AddProductionEncryptionAndSigningCertificate(
        //            "openiddict.pfx",
        //            "9b26e3c2-ae0a-4c47-bb89-606fcd97cf0b"
        //        );
        //        serverBuilder.SetIssuer(new Uri(configuration["AuthServer:Authority"]!));
        //    });
        //}
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
        ConfigureAuthentication(context, configuration);
        ConfigureUrls(configuration);
        ConfigureBundles();
        ConfigureAutoMapper();
        ConfigureVirtualFileSystem(hostingEnvironment);
        ConfigureSwaggerServices(context.Services);
        ConfigureAutoApiControllers();
        ConfigMenu();

        Configure<AbpMultiTenancyOptions>(options =>
        {
            options.IsEnabled = MultiTenancyConsts.IsEnabled;
        });
    }

    private void ConfigureAuthentication(
        ServiceConfigurationContext context,
        IConfiguration configuration
    )
    {
        context.Services.ForwardIdentityAuthenticationForBearer(
            OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme
        );
        context.Services.Configure<AbpClaimsPrincipalFactoryOptions>(options =>
        {
            options.IsDynamicClaimsEnabled = true;
        });
    }

    private void ConfigureUrls(IConfiguration configuration)
    {
        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
            options.RedirectAllowedUrls.AddRange(
                configuration["App:RedirectAllowedUrls"]?.Split(',') ?? Array.Empty<string>()
            );
        });
    }

    private void ConfigureBundles()
    {
        Configure<AbpBundlingOptions>(options => { });
    }

    private void ConfigureVirtualFileSystem(IWebHostEnvironment hostingEnvironment)
    {
        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<CRMDomainSharedModule>(
                    Path.Combine(
                        hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}CRM.Domain.Shared"
                    )
                );
                options.FileSets.ReplaceEmbeddedByPhysical<CRMDomainModule>(
                    Path.Combine(
                        hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}CRM.Domain"
                    )
                );
                options.FileSets.ReplaceEmbeddedByPhysical<CRMApplicationContractsModule>(
                    Path.Combine(
                        hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}CRM.Application.Contracts"
                    )
                );
                options.FileSets.ReplaceEmbeddedByPhysical<CRMApplicationModule>(
                    Path.Combine(
                        hostingEnvironment.ContentRootPath,
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
        Configure<AbpRadzenUIOptions>(options =>
        {
            options.RouterAdditionalAssemblies = [typeof(Home).Assembly];
        });
    }

    private void ConfigMenu()
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new CRMMenuContributor());
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var env = context.GetEnvironment();
        var app = context.GetApplicationBuilder();

        //if (env.IsDevelopment())
        //{
        //    app.UseDeveloperExceptionPage();
        //}

        app.UseAbpRequestLocalization();

        if (!env.IsDevelopment())
        {
            app.UseErrorPage();
            app.UseHsts();
        }

        app.UseCorrelationId();
        app.UseRouting();
        app.UseStaticFiles();
        app.UseStatusCodePagesWithRedirects("/404");
        app.UseAntiforgery();
        ((WebApplication)app)
            .MapRazorComponents<App>()
            .AddAdditionalAssemblies(typeof(Home).Assembly)
            .AddInteractiveServerRenderMode();
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
        app.UseConfiguredEndpoints();
    }
}
