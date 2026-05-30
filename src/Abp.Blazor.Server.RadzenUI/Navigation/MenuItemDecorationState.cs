namespace Abp.RadzenUI.Navigation;

public sealed class MenuItemDecorationState
{
    public event Func<MenuItemDecorationChange, Task>? Changed;

    public async Task NotifyChangedAsync(params string[] refreshKeys)
    {
        if (Changed is null)
        {
            return;
        }

        var normalizedKeys = refreshKeys
            .Where(static key => !string.IsNullOrWhiteSpace(key))
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        var change = new MenuItemDecorationChange(normalizedKeys);

        foreach (var handler in Changed.GetInvocationList().Cast<Func<MenuItemDecorationChange, Task>>())
        {
            await handler(change);
        }
    }
}