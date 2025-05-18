using Abp.RadzenUI.Components.Pages.Setting;
using Abp.RadzenUI.Localization;
using Abp.RadzenUI.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Abp.RadzenUI.Blazor.SettingManagement;

public class AccountPageContributor : ISettingComponentContributor
{
    public async Task ConfigureAsync(SettingComponentCreationContext context)
    {
        var l = context.ServiceProvider.GetRequiredService<IStringLocalizer<AbpRadzenUIResource>>();
        if (await CheckPermissionsAsync(context))
        {
            context.Groups.Add(
                new SettingComponentGroup(
                    "AbpRadzenUI.AccountSetting",
                    l["Menu:AccountSetting"],
                    typeof(AccountSettingComponent)
                )
            );
        }
    }

    public async Task<bool> CheckPermissionsAsync(SettingComponentCreationContext context)
    {
        var authorizationService =
            context.ServiceProvider.GetRequiredService<IAuthorizationService>();

        return await authorizationService.IsGrantedAsync(
            SettingManagementExtensionPermissions.Account
        );
    }
}
