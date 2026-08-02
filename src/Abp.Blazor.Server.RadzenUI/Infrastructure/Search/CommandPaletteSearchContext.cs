namespace Abp.RadzenUI.Infrastructure.Search;

/// <summary>
/// Input passed to every <see cref="ICommandPaletteContributor"/> for a single query.
/// </summary>
public sealed class CommandPaletteSearchContext
{
    /// <summary>The trimmed keyword the user typed. Never null; may be empty.</summary>
    public required string Keyword { get; init; }

    /// <summary>Upper bound on results a single contributor should return.</summary>
    public int MaxResultsPerGroup { get; init; } = 8;

    /// <summary>
    /// Cancels the search when the keyword changes or the palette closes. Async
    /// contributors (Phase 2 entity search) should honor it.
    /// </summary>
    public CancellationToken CancellationToken { get; init; }
}
