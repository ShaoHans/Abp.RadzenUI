using Volo.Abp.DependencyInjection;

namespace Abp.RadzenUI.Navigation;

public interface IMenuItemDecorationContributor : ITransientDependency
{
    Task ConfigureAsync(MenuItemDecorationContext context);
}