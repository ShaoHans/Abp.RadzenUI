namespace Abp.RadzenUI.Components.Shared;

/// <summary>
/// A single row action rendered by <c>RowActionMenu</c>.
/// </summary>
/// <remarks>
/// Deliberately pure data - it carries no callback. The owning page receives the
/// <see cref="Key"/> through <c>RowActionMenu.OnAction</c> and dispatches from
/// there, which keeps the page the event receiver (so Blazor re-renders it after
/// an action runs) and keeps this type trivial to build and assert on.
/// </remarks>
/// <param name="Key">Stable identifier reported back through <c>RowActionMenu.OnAction</c>.</param>
/// <param name="Text">Localized label, also used as the tooltip when rendered as a bare icon button.</param>
/// <param name="Icon">Material symbol name.</param>
/// <param name="IconColor">
/// Optional icon color. Leave <c>null</c> so the icon inherits the theme-aware menu
/// text color: Radzen's semantic tokens are shared between the light and dark themes
/// (<c>--rz-warning</c> is <c>#ff9800</c> in both), so a fixed color is illegible in
/// one of them. Reserve it for destructive actions (<c>Colors.Danger</c>).
/// </param>
/// <param name="IsPrimary">
/// Whether this action is promoted to the split button's main button. At most one
/// action should set it; when none does, every action lands in an overflow menu.
/// </param>
public sealed record RowAction(
    string Key,
    string Text,
    string Icon,
    string? IconColor = null,
    bool IsPrimary = false
);
