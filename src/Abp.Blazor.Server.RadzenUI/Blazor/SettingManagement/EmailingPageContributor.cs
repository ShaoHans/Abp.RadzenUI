using Abp.RadzenUI.Components.Pages.Setting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Volo.Abp.Features;
using Volo.Abp.MultiTenancy;
using Volo.Abp.SettingManagement;
using Volo.Abp.SettingManagement.Localization;

namespace Abp.RadzenUI.Blazor.SettingManagement;

public class EmailingPageContributor : ISettingComponentContributor
{
    public async Task ConfigureAsync(SettingComponentCreationContext context)
    {
        if (!await CheckPermissionsInternalAsync(context))
        {
            return;
        }

        var l = context.ServiceProvider.GetRequiredService<IStringLocalizer<AbpSettingManagementResource>>();
        context.Groups.Add(
            new SettingComponentGroup(
                "Volo.Abp.SettingManagement",
                l["Menu:Emailing"],
                typeof(EmailSettingComponent)
            )
        );
    }

    public async Task<bool> CheckPermissionsAsync(SettingComponentCreationContext context)
    {
        return await CheckPermissionsInternalAsync(context);
    }

    private static async Task<bool> CheckPermissionsInternalAsync(SettingComponentCreationContext context)
    {
        if (!await CheckFeatureAsync(context))
        {
            return false;
        }

        var authorizationService = context.ServiceProvider.GetRequiredService<IAuthorizationService>();

        return await authorizationService.IsGrantedAsync(SettingManagementPermissions.Emailing);
    }

    private static async Task<bool> CheckFeatureAsync(SettingComponentCreationContext context)
    {
        var currentTenant = context.ServiceProvider.GetRequiredService<ICurrentTenant>();

        if (!currentTenant.IsAvailable)
        {
            return true;
        }

        var featureCheck = context.ServiceProvider.GetRequiredService<IFeatureChecker>();

        return await featureCheck.IsEnabledAsync(SettingManagementFeatures.AllowChangingEmailSettings);
    }
}
