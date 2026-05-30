namespace Abp.RadzenUI.Navigation;

public sealed class MenuItemDecorationChange(IReadOnlyCollection<string> refreshKeys)
{
    public IReadOnlyCollection<string> RefreshKeys { get; } = refreshKeys;

    public bool RefreshAll => RefreshKeys.Count == 0;
}