namespace Abp.RadzenUI.Infrastructure.Search;

/// <summary>
/// One selectable scope in the command palette. Each tab maps to a
/// <see cref="ICommandPaletteContributor.GroupKey"/>; only the active tab's contributor(s)
/// run for a query.
/// </summary>
public sealed record CommandPaletteTab
{
    public required string Key { get; init; }

    /// <summary>Localized tab label.</summary>
    public required string DisplayName { get; init; }

    /// <summary>Optional Material Symbols icon for the tab.</summary>
    public string? Icon { get; init; }

    public required int Order { get; init; }
}
