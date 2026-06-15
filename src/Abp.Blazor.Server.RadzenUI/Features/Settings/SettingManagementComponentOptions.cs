namespace Abp.RadzenUI.Features.Settings;

public class SettingManagementComponentOptions
{
    public List<ISettingComponentContributor> Contributors { get; }

    public SettingManagementComponentOptions()
    {
        Contributors = [];
    }
}
