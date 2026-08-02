namespace Abp.RadzenUI.Infrastructure.Search;

/// <summary>
/// A single result rendered inside the command palette (Ctrl+K global search).
/// Every result ultimately navigates to a <see cref="Url"/>, so both page jumps
/// and future entity results share the same shape.
/// </summary>
public sealed record CommandPaletteItem
{
    /// <summary>Primary text shown for the result.</summary>
    public required string Title { get; init; }

    /// <summary>Secondary text, e.g. the breadcrumb trail "Administration › Identity".</summary>
    public string? Description { get; init; }

    /// <summary>Radzen/Material Symbols icon name.</summary>
    public string? Icon { get; init; }

    /// <summary>Optional icon color.</summary>
    public string? IconColor { get; init; }

    /// <summary>Navigation target invoked when the result is selected.</summary>
    public required string Url { get; init; }

    /// <summary>
    /// Whether <see cref="Url"/> should trigger a full page reload. Menu jumps use
    /// client-side navigation; some external links may prefer a reload.
    /// </summary>
    public bool ForceLoad { get; init; }

    /// <summary>
    /// Higher scores rank first inside a group. Contributors set this from match
    /// quality (prefix &gt; word-start &gt; substring).
    /// </summary>
    public int Score { get; init; }
}
