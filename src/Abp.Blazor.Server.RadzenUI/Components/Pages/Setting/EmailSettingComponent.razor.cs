using Abp.RadzenUI.Models;
using Microsoft.AspNetCore.Components;
using Radzen;
using Volo.Abp.AspNetCore.Components.Web.Configuration;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.SettingManagement;
using Volo.Abp.SettingManagement.Localization;

namespace Abp.RadzenUI.Components.Pages.Setting;

public partial class EmailSettingComponent
{
    [Inject]
    protected IEmailSettingsAppService EmailSettingsAppService { get; set; } = default!;

    [Inject]
    protected IPermissionChecker PermissionChecker { get; set; } = default!;

    [Inject]
    private ICurrentApplicationConfigurationCacheResetService CurrentApplicationConfigurationCacheResetService { get; set; } =
        default!;

    [Inject]
    protected DialogService DialogService { get; set; } = default!;

    protected UpdateEmailSettingsVM EmailSettings = new();

    protected SendTestEmailVM SendTestEmailInput = new();

    protected bool HasSendTestEmailPermission { get; set; }

    public EmailSettingComponent()
    {
        LocalizationResource = typeof(AbpSettingManagementResource);
    }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            EmailSettings = ObjectMapper.Map<EmailSettingsDto, UpdateEmailSettingsVM>(
                await EmailSettingsAppService.GetAsync()
            );
            HasSendTestEmailPermission = await PermissionChecker.IsGrantedAsync(
                SettingManagementPermissions.EmailingTest
            );
            SendTestEmailInput = new SendTestEmailVM();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    protected virtual async Task UpdateSettingsAsync()
    {
        try
        {
            await EmailSettingsAppService.UpdateAsync(
                ObjectMapper.Map<UpdateEmailSettingsVM, UpdateEmailSettingsDto>(EmailSettings)
            );

            await CurrentApplicationConfigurationCacheResetService.ResetAsync();

            await Message.Success(L["SavedSuccessfully"]);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    protected virtual async Task OpenSendTestEmailModalAsync()
    {
        try
        {
            await DialogService.OpenAsync<SendTestEmail>(
                L["SendTestEmail"],
                parameters: new Dictionary<string, object>
                {
                    { "DefaultFromAddress",EmailSettings.DefaultFromAddress}
                },
                options: new DialogOptions
                {
                    Draggable = true,
                    Width = "600px",
                    Height = "550px"
                }
            );
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }
}
