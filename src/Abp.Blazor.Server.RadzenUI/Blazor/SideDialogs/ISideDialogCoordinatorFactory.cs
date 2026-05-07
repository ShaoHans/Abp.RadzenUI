namespace Abp.RadzenUI;

public interface ISideDialogCoordinatorFactory
{
    SideDialogCoordinator<T> Create<T>()
        where T : class;
}
