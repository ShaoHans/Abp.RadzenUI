using System.ComponentModel.DataAnnotations;
using Abp.RadzenUI.Models;
using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Messages;
using Volo.Abp.AspNetCore.Components.Web.Configuration;
using Volo.Abp.Auditing;
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

    //protected virtual async Task OpenSendTestEmailModalAsync()
    //{
    //    try
    //    {
    //        var emailSettings = await EmailSettingsAppService.GetAsync();
    //        SendTestEmailInput = new SendTestEmailVM
    //        {
    //            SenderEmailAddress = emailSettings.DefaultFromAddress,
    //            TargetEmailAddress = CurrentUser.Email!,
    //            Subject = L["TestEmailSubject", new Random().Next(1000, 9999)],
    //            Body = L["TestEmailBody"]
    //        };

    //        await SendTestEmailModal.Show();
    //    }
    //    catch (Exception ex)
    //    {
    //        await HandleErrorAsync(ex);
    //    }
    //}

    //protected virtual Task CloseSendTestEmailModalAsync()
    //{
    //    return InvokeAsync(SendTestEmailModal.Hide);
    //}

    protected virtual async Task SendTestEmailAsync()
    {
        try
        {
            await EmailSettingsAppService.SendTestEmailAsync(
                ObjectMapper.Map<SendTestEmailVM, SendTestEmailInput>(SendTestEmailInput)
            );

            await Message.Success(L["SentSuccessfully"]);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }
}
