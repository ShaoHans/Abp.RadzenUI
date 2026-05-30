using Radzen;

namespace Abp.RadzenUI.Navigation;

public class MenuItemBadgeDefinition
{
    public string Text { get; set; } = string.Empty;

    public BadgeStyle BadgeStyle { get; set; } = BadgeStyle.Light;

    public bool Visible { get; set; } = true;

    public int Order { get; set; }

    public string? Style { get; set; }
}