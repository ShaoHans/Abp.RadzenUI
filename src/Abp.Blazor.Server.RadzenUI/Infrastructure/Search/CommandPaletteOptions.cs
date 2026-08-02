namespace Abp.RadzenUI.Infrastructure.Search;

/// <summary>
/// Configures the command palette (Ctrl+K global search). Contributors are stored as
/// types and resolved from DI per query so they can consume scoped services.
/// </summary>
public class CommandPaletteOptions
{
    /// <summary>Master switch. When false the palette is not wired up.</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Minimum keyword length before contributors are queried. Menu jumps can start
    /// at 1 character; heavier entity contributors may want to enforce more.
    /// </summary>
    public int MinKeywordLength { get; set; } = 1;

    /// <summary>Max results shown per group.</summary>
    public int MaxResultsPerGroup { get; set; } = 8;

    /// <summary>
    /// Contributor types, each implementing <see cref="ICommandPaletteContributor"/>.
    /// Registered in DI by the module and resolved from the current circuit scope.
    /// </summary>
    public List<Type> Contributors { get; } = [];

    public void AddContributor<TContributor>()
        where TContributor : ICommandPaletteContributor
    {
        if (!Contributors.Contains(typeof(TContributor)))
        {
            Contributors.Add(typeof(TContributor));
        }
    }
}
