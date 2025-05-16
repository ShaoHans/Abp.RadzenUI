namespace Abp.RadzenUI.Blazor.SettingManagement;

public class SettingManagementComponentOptions
{
    public List<ISettingComponentContributor> Contributors { get; }

    public SettingManagementComponentOptions()
    {
        Contributors = [];
    }
}
