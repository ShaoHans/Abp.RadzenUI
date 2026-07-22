namespace Abp.RadzenUI.Infrastructure.Search;

/// <summary>
/// Backs the command palette UI. Each registered <see cref="ICommandPaletteContributor"/>
/// surfaces as a selectable tab; only the active tab's contributor runs for a query, so no
/// contributor executes unless the user picks its tab.
/// </summary>
public interface ICommandPaletteManager
{
    bool Enabled { get; }

    int MinKeywordLength { get; }

    /// <summary>Tabs to show, ordered by <see cref="ICommandPaletteContributor.Order"/>.</summary>
    IReadOnlyList<CommandPaletteTab> GetTabs();

    /// <summary>Runs only the contributor(s) mapped to <paramref name="tabKey"/>.</summary>
    Task<IReadOnlyList<CommandPaletteItem>> SearchAsync(
        string tabKey,
        string keyword,
        CancellationToken cancellationToken = default);
}
