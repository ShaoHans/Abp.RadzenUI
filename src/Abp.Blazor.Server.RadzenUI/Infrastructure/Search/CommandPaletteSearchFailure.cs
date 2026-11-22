namespace Abp.RadzenUI.Infrastructure.Search;

/// <summary>
/// An empty result that means "the search failed". A list, so that the signature of
/// <see cref="ICommandPaletteManager.SearchAsync"/> stays the same for its callers.
/// </summary>
internal sealed class CommandPaletteSearchFailure : List<CommandPaletteItem>;
