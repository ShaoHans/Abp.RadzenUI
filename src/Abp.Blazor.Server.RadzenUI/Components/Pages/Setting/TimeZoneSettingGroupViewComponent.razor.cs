using Microsoft.AspNetCore.Components;
using Volo.Abp;
using Volo.Abp.AspNetCore.Components.Messages;
using Volo.Abp.AspNetCore.Components.Web.Configuration;
using Volo.Abp.SettingManagement;
using Volo.Abp.SettingManagement.Localization;

namespace Abp.RadzenUI.Components.Pages.Setting;

public partial class TimeZoneSettingGroupViewComponent
{
    [Inject]
    protected ITimeZoneSettingsAppService TimeZoneSettingsAppService { get; set; } = default!;

    [Inject]
    private ICurrentApplicationConfigurationCacheResetService CurrentApplicationConfigurationCacheResetService { get; set; } = default!;

    [Inject]
    protected IUiMessageService UiMessageService { get; set; } = default!;

    protected UpdateTimezoneSettingsViewModel TimezoneSettings = new();

    public TimeZoneSettingGroupViewComponent()
    {
        LocalizationResource = typeof(AbpSettingManagementResource);
    }

    protected async override Task OnInitializedAsync()
    {
        TimezoneSettings = new UpdateTimezoneSettingsViewModel()
        {
            Timezone = await TimeZoneSettingsAppService.GetAsync(),
            TimeZoneItems = await TimeZoneSettingsAppService.GetTimezonesAsync()
        };
    }

    protected virtual async Task OnSelectedValueChangedAsync(string timezone)
    {
        TimezoneSettings.Timezone = timezone;
        await InvokeAsync(StateHasChanged);
    }

    protected virtual async Task UpdateSettingsAsync()
    {
        try
        {
            await TimeZoneSettingsAppService.UpdateAsync(TimezoneSettings.Timezone);
            await CurrentApplicationConfigurationCacheResetService.ResetAsync();
            await Notify.Success(L["SavedSuccessfully"]);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    public class UpdateTimezoneSettingsViewModel
    {
        public string Timezone { get; set; } = default!;

        public List<NameValue> TimeZoneItems { get; set; } = [];
    }
}