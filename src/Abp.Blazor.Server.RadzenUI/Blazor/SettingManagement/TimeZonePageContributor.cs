using Abp.RadzenUI.Components.Pages.Setting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Volo.Abp.SettingManagement;
using Volo.Abp.SettingManagement.Localization;
using Volo.Abp.Timing;

namespace Abp.RadzenUI.Blazor.SettingManagement;

public class TimeZonePageContributor : ISettingComponentContributor
{
    public async Task ConfigureAsync(SettingComponentCreationContext context)
    {
        var l = context.ServiceProvider.GetRequiredService<IStringLocalizer<AbpSettingManagementResource>>();
        if (await CheckPermissionsAsync(context))
        {
            context.Groups.Add(
                new SettingComponentGroup(
                    "Volo.Abp.TimeZone",
                    l["Menu:TimeZone"],
                    typeof(TimeZoneSettingComponent)
                )
            );
        }
    }

    public async Task<bool> CheckPermissionsAsync(SettingComponentCreationContext context)
    {
        var authorizationService = context.ServiceProvider.GetRequiredService<IAuthorizationService>();

        return context.ServiceProvider.GetRequiredService<IClock>().SupportsMultipleTimezone
            && await authorizationService.IsGrantedAsync(SettingManagementPermissions.TimeZone);
    }
}