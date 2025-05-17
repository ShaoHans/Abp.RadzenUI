using Abp.RadzenUI.Application.Contracts.Settings;
using Volo.Abp.Account.Settings;
using Volo.Abp.MultiTenancy;
using Volo.Abp.SettingManagement;

namespace Abp.RadzenUI.Application;

public class AccountSettingsAppService(ISettingManager settingManager)
    : SettingManagementAppServiceBase,
        IAccountSettingsAppService
{
    protected ISettingManager SettingManager { get; } = settingManager;

    public async Task<AccountSettingsDto> GetAsync()
    {
        var settingsDto = new AccountSettingsDto
        {
            IsSelfRegistrationEnabled = Convert.ToBoolean(
                await SettingProvider.GetOrNullAsync(AccountSettingNames.IsSelfRegistrationEnabled)
            ),
            EnableLocalLogin = Convert.ToBoolean(
                await SettingProvider.GetOrNullAsync(AccountSettingNames.EnableLocalLogin)
            )
        };

        if (CurrentTenant.IsAvailable)
        {
            settingsDto.IsSelfRegistrationEnabled = Convert.ToBoolean(
                await SettingManager.GetOrNullForTenantAsync(
                    AccountSettingNames.IsSelfRegistrationEnabled,
                    CurrentTenant.GetId(),
                    false
                )
            );
            settingsDto.EnableLocalLogin = Convert.ToBoolean(
                await SettingManager.GetOrNullForTenantAsync(
                    AccountSettingNames.EnableLocalLogin,
                    CurrentTenant.GetId(),
                    false
                )
            );
        }

        return settingsDto;
    }

    public async Task UpdateAsync(UpdateAccountSettingsDto input)
    {
        await SettingManager.SetForTenantOrGlobalAsync(
            CurrentTenant.Id,
            AccountSettingNames.IsSelfRegistrationEnabled,
            input.IsSelfRegistrationEnabled.ToString()
        );
        await SettingManager.SetForTenantOrGlobalAsync(
            CurrentTenant.Id,
            AccountSettingNames.EnableLocalLogin,
            input.EnableLocalLogin.ToString()
        );
    }
}
