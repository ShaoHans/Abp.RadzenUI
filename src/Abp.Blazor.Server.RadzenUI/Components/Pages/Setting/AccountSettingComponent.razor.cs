using Abp.RadzenUI.Application.Contracts.Settings;
using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Web.Configuration;
using Volo.Abp.SettingManagement.Localization;

namespace Abp.RadzenUI.Components.Pages.Setting;

public partial class AccountSettingComponent
{
    [Inject]
    protected IAccountSettingsAppService AccountSettingsAppService { get; set; } = default!;

    [Inject]
    private ICurrentApplicationConfigurationCacheResetService CurrentApplicationConfigurationCacheResetService { get; set; } =
        default!;

    protected AccountSettingsDto AccountSettings = new();

    public AccountSettingComponent()
    {
        LocalizationResource = typeof(AbpSettingManagementResource);
    }

    protected override async Task OnInitializedAsync()
    {
        AccountSettings = await AccountSettingsAppService.GetAsync();
    }

    protected virtual async Task UpdateSettingsAsync()
    {
        try
        {
            await AccountSettingsAppService.UpdateAsync(
                new UpdateAccountSettingsDto
                {
                    IsSelfRegistrationEnabled = AccountSettings.IsSelfRegistrationEnabled,
                    EnableLocalLogin = AccountSettings.EnableLocalLogin
                }
            );
            await CurrentApplicationConfigurationCacheResetService.ResetAsync();
            await Message.Success(L["SavedSuccessfully"]);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }
}
