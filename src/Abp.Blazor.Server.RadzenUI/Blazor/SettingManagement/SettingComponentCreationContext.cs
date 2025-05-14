using Volo.Abp.DependencyInjection;

namespace Abp.RadzenUI.Blazor.SettingManagement;

public class SettingComponentCreationContext(IServiceProvider serviceProvider)
    : IServiceProviderAccessor
{
    public IServiceProvider ServiceProvider { get; } = serviceProvider;

    public List<SettingComponentGroup> Groups { get; private set; } = [];

    public void Normalize()
    {
        Order();
    }

    private void Order()
    {
        Groups = [.. Groups.OrderBy(item => item.Order).ThenBy(item => item.DisplayName)];
    }
}
