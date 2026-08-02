using Volo.Abp.DependencyInjection;

namespace Abp.RadzenUI.Features.Settings;

public class SettingComponentCreationContext(IServiceProvider serviceProvider)
    : IServiceProviderAccessor
{
    public IServiceProvider ServiceProvider { get; } = serviceProvider;

    public List<SettingComponentGroup> Groups { get; private set; } = [];

    public void Normalize()
    {
        Groups = [.. Groups.OrderBy(item => item.Order).ThenBy(item => item.DisplayName)];
    }
}
