namespace Abp.RadzenUI.Features.Settings;

public interface ISettingComponentContributor
{
    Task ConfigureAsync(SettingComponentCreationContext context);

    Task<bool> CheckPermissionsAsync(SettingComponentCreationContext context);
}
