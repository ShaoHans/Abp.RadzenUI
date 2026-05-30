using Volo.Abp.UI.Navigation;

namespace Abp.RadzenUI.Navigation;

public class MenuItemDecorationContext(ApplicationMenuItem menuItem)
{
    public ApplicationMenuItem MenuItem { get; } = menuItem;

    public string DisplayName { get; set; } = menuItem.DisplayName;

    public List<MenuItemBadgeDefinition> Badges { get; } = [];

    public HashSet<string> RefreshKeys { get; } = new(StringComparer.Ordinal);

    public bool HasVisibleBadges => Badges.Any(static badge => badge.Visible);
}