using Abp.RadzenUI;
using Abp.RadzenUI.Localization;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Localization;
using Volo.Abp.UI.Navigation;

namespace RadzenUiTemplateNamespace.RadzenUI;

public static class RadzenUiTemplateRadzenUIIntegrationExtensions
{
    public static IServiceCollection AddRadzenUIIntegration<TPageAssemblyMarker, TLocalizationResource, TMenuContributor>(
        this IServiceCollection services)
        where TLocalizationResource : class
        where TMenuContributor : IMenuContributor, new()
    {
        services.Configure<AbpRadzenUIOptions>(options =>
        {
            options.RouterAdditionalAssemblies = [typeof(TPageAssemblyMarker).Assembly];

            options.TitleBar.Title = "RadzenUiTemplateTitle";
            options.LoginPage.Title = "RadzenUiTemplateTitle";
            options.Theme.EnablePremiumTheme = bool.Parse("RadzenUiTemplateEnablePremiumTheme");
        });

        services.Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<TLocalizationResource>()
                .AddBaseTypes(typeof(AbpRadzenUIResource));
        });

        services.Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new TMenuContributor());
        });

        return services;
    }
}
