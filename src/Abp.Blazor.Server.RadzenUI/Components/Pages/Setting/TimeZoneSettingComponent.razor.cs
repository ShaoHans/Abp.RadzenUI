using Abp.RadzenUI.Models;
using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Web.Configuration;
using Volo.Abp.SettingManagement;
using Volo.Abp.SettingManagement.Localization;

namespace Abp.RadzenUI.Components.Pages.Setting;

public partial class TimeZoneSettingComponent
{
    [Inject]
    protected ITimeZoneSettingsAppService TimeZoneSettingsAppService { get; set; } = default!;

    [Inject]
    private ICurrentApplicationConfigurationCacheResetService CurrentApplicationConfigurationCacheResetService { get; set; } = default!;

    protected UpdateTimezoneSettingsVm TimezoneSettings = new();

    public TimeZoneSettingComponent()
    {
        LocalizationResource = typeof(AbpSettingManagementResource);
    }

    protected async override Task OnInitializedAsync()
    {
        TimezoneSettings = new UpdateTimezoneSettingsVm()
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
            await Message.Success(L["SavedSuccessfully"]);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }
}