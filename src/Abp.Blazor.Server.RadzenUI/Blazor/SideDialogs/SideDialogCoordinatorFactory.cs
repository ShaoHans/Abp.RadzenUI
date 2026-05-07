using Microsoft.Extensions.DependencyInjection;

namespace Abp.RadzenUI;

public sealed class SideDialogCoordinatorFactory(IServiceProvider serviceProvider)
    : ISideDialogCoordinatorFactory
{
    public SideDialogCoordinator<T> Create<T>()
        where T : class
    {
        return ActivatorUtilities.CreateInstance<SideDialogCoordinator<T>>(serviceProvider);
    }
}
