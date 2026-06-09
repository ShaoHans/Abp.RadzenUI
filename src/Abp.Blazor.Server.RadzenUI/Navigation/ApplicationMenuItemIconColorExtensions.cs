using Volo.Abp.UI.Navigation;

namespace Abp.RadzenUI.Navigation;

public static class ApplicationMenuItemIconColorExtensions
{
    public const string IconColorCustomDataKey = "Abp.RadzenUI.IconColor";

    public static ApplicationMenuItem WithIconColor(this ApplicationMenuItem menuItem, string iconColor)
    {
        ArgumentNullException.ThrowIfNull(menuItem);

        if (!string.IsNullOrWhiteSpace(iconColor))
        {
            menuItem.WithCustomData(IconColorCustomDataKey, iconColor);
        }

        return menuItem;
    }

    public static string? GetIconColor(this ApplicationMenuItem menuItem)
    {
        ArgumentNullException.ThrowIfNull(menuItem);

        return menuItem.CustomData.TryGetValue(IconColorCustomDataKey, out var value)
            ? value?.ToString()
            : null;
    }
}
