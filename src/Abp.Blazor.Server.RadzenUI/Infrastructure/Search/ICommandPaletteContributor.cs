namespace Abp.RadzenUI.Infrastructure.Search;

/// <summary>
/// Extension point that feeds one group of results into the command palette (Ctrl+K).
/// Phase 1 ships <see cref="MenuCommandPaletteContributor"/> for page jumps; host apps
/// can register additional contributors (e.g. entity search) via
/// <see cref="CommandPaletteOptions.Contributors"/> without touching the palette UI.
///
/// Contributors are resolved from DI per query, so they may depend on scoped services.
/// </summary>
public interface ICommandPaletteContributor
{
    /// <summary>
    /// Stable identifier for the group. Results from contributors sharing a key are
    /// merged into one group. Not shown to the user — see <see cref="GroupDisplayName"/>.
    /// </summary>
    string GroupKey { get; }

    /// <summary>
    /// Localized group heading shown to the user. The contributor localizes this itself
    /// (against its own resource), so host groups don't depend on the theme resource.
    /// When contributors share a <see cref="GroupKey"/>, the first one's name wins.
    /// </summary>
    string GroupDisplayName { get; }

    /// <summary>
    /// Optional Material Symbols icon for the tab (e.g. "inventory_2"). Null shows no icon.
    /// Default is null so existing contributors need no change.
    /// </summary>
    string? GroupIcon => null;

    /// <summary>Display order of the group; lower comes first.</summary>
    int Order { get; }

    /// <summary>
    /// Returns the matching results for the given query. Should return an empty list
    /// (not throw) when nothing matches or the keyword is too short for this source.
    /// </summary>
    Task<IReadOnlyList<CommandPaletteItem>> SearchAsync(CommandPaletteSearchContext context);
}
