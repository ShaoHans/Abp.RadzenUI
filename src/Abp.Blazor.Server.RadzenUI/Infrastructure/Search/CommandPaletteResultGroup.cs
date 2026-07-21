namespace Abp.RadzenUI.Infrastructure.Search;

/// <summary>
/// A localized, ordered group of results as surfaced to the palette UI.
/// </summary>
public sealed record CommandPaletteResultGroup
{
    public required string GroupKey { get; init; }

    /// <summary>Localized heading text.</summary>
    public required string DisplayName { get; init; }

    public required int Order { get; init; }

    public required IReadOnlyList<CommandPaletteItem> Items { get; init; }
}
