namespace Abp.RadzenUI.Infrastructure.Search;

/// <summary>
/// Aggregates all registered <see cref="ICommandPaletteContributor"/> results for a query
/// and exposes them to the palette UI as localized, ordered groups.
/// </summary>
public interface ICommandPaletteManager
{
    bool Enabled { get; }

    int MinKeywordLength { get; }

    Task<IReadOnlyList<CommandPaletteResultGroup>> SearchAsync(
        string keyword,
        CancellationToken cancellationToken = default);
}
