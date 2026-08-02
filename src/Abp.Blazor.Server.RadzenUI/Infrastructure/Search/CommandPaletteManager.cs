using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace Abp.RadzenUI.Infrastructure.Search;

public class CommandPaletteManager(
    IServiceProvider serviceProvider,
    IOptions<CommandPaletteOptions> options,
    ILogger<CommandPaletteManager> logger)
    : ICommandPaletteManager, IScopedDependency
{
    readonly CommandPaletteOptions _options = options.Value;

    public bool Enabled => _options.Enabled && _options.Contributors.Count > 0;

    public int MinKeywordLength => _options.MinKeywordLength;

    public IReadOnlyList<CommandPaletteTab> GetTabs()
    {
        if (!Enabled)
        {
            return [];
        }

        var tabs = new Dictionary<string, CommandPaletteTab>();

        foreach (var contributor in ResolveContributors())
        {
            // First contributor for a given key defines the tab's label/order.
            if (!tabs.ContainsKey(contributor.GroupKey))
            {
                tabs[contributor.GroupKey] = new CommandPaletteTab
                {
                    Key = contributor.GroupKey,
                    DisplayName = contributor.GroupDisplayName,
                    Icon = contributor.GroupIcon,
                    Order = contributor.Order,
                };
            }
        }

        return tabs.Values.OrderBy(static t => t.Order).ToList();
    }

    public async Task<IReadOnlyList<CommandPaletteItem>> SearchAsync(
        string tabKey,
        string keyword,
        CancellationToken cancellationToken = default)
    {
        keyword = keyword?.Trim() ?? string.Empty;

        if (!Enabled || string.IsNullOrEmpty(tabKey) || keyword.Length < _options.MinKeywordLength)
        {
            return [];
        }

        var context = new CommandPaletteSearchContext
        {
            Keyword = keyword,
            MaxResultsPerGroup = _options.MaxResultsPerGroup,
            CancellationToken = cancellationToken,
        };

        var results = new List<CommandPaletteItem>();

        // Only the contributor(s) belonging to the active tab run — never all of them.
        foreach (var contributor in ResolveContributors())
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!string.Equals(contributor.GroupKey, tabKey, StringComparison.Ordinal))
            {
                continue;
            }

            try
            {
                var items = await contributor.SearchAsync(context);
                results.AddRange(items);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                // A single failing contributor must not break the palette.
                logger.LogError(ex,
                    "Command palette contributor {Type} threw and was skipped.",
                    contributor.GetType().FullName);
            }
        }

        return results
            .OrderByDescending(static i => i.Score)
            .Take(_options.MaxResultsPerGroup)
            .ToList();
    }

    IEnumerable<ICommandPaletteContributor> ResolveContributors()
    {
        foreach (var contributorType in _options.Contributors)
        {
            if (serviceProvider.GetService(contributorType) is ICommandPaletteContributor contributor)
            {
                yield return contributor;
            }
            else
            {
                logger.LogWarning(
                    "Command palette contributor {Type} is not registered in DI and was skipped.",
                    contributorType.FullName);
            }
        }
    }
}
