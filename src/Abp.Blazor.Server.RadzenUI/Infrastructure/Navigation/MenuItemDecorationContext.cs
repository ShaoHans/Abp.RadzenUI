using Volo.Abp.UI.Navigation;

namespace Abp.RadzenUI.Infrastructure.Navigation;

public class MenuItemDecorationContext(ApplicationMenuItem menuItem)
{
    public ApplicationMenuItem MenuItem { get; } = menuItem;

    public string DisplayName { get; set; } = menuItem.DisplayName;

    public string? IconColor { get; set; } = menuItem.GetIconColor();

    public List<MenuItemBadgeDefinition> Badges { get; } = [];

    public HashSet<string> RefreshKeys { get; } = new(StringComparer.Ordinal);

    public bool HasVisibleBadges => Badges.Any(static badge => badge.Visible);
}
