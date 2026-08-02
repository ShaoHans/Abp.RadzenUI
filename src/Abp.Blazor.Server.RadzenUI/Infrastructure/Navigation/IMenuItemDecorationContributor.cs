using Volo.Abp.DependencyInjection;

namespace Abp.RadzenUI.Infrastructure.Navigation;

public interface IMenuItemDecorationContributor : ITransientDependency
{
    Task ConfigureAsync(MenuItemDecorationContext context);
}