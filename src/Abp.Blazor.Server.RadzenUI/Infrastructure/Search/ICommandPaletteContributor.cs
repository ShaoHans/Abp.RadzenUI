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
    /// Localization key for the group heading (looked up against the theme resource).
    /// Results from contributors sharing a key are merged into one group.
    /// </summary>
    string GroupKey { get; }

    /// <summary>Display order of the group; lower comes first.</summary>
    int Order { get; }

    /// <summary>
    /// Returns the matching results for the given query. Should return an empty list
    /// (not throw) when nothing matches or the keyword is too short for this source.
    /// </summary>
    Task<IReadOnlyList<CommandPaletteItem>> SearchAsync(CommandPaletteSearchContext context);
}
