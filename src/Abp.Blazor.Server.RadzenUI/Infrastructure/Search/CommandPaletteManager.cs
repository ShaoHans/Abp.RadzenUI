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
            try
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
            catch (Exception ex)
            {
                LogSkipped(ex, contributor.GetType());
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
        var searched = false;
        var failed = false;

        // Only the contributor(s) belonging to the active tab run — never all of them.
        foreach (var contributor in ResolveContributors())
        {
            cancellationToken.ThrowIfCancellationRequested();

            var inTab = false;
            try
            {
                inTab = string.Equals(contributor.GroupKey, tabKey, StringComparison.Ordinal);
                if (!inTab)
                {
                    continue;
                }

                searched = true;
                var items = await contributor.SearchAsync(context);
                results.AddRange(items);
            }
            // Only our own cancellation; a timeout inside the contributor is a failure.
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                // A single failing contributor must not break the palette.
                failed |= inTab;
                LogSkipped(ex, contributor.GetType());
            }
        }

        // Nothing to show because the tab's source is broken, which is not "no results".
        if (results.Count == 0 && (failed || !searched))
        {
            return new CommandPaletteSearchFailure();
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
            if (TryResolve(contributorType) is { } contributor)
            {
                yield return contributor;
            }
        }
    }

    ICommandPaletteContributor? TryResolve(Type contributorType)
    {
        try
        {
            if (serviceProvider.GetService(contributorType) is ICommandPaletteContributor contributor)
            {
                return contributor;
            }

            logger.LogWarning(
                "Command palette contributor {Type} is not registered in DI and was skipped.",
                contributorType.FullName);
        }
        catch (Exception ex)
        {
            LogSkipped(ex, contributorType);
        }

        return null;
    }

    void LogSkipped(Exception exception, Type contributorType) =>
        logger.LogError(exception,
            "Command palette contributor {Type} threw and was skipped.",
            contributorType.FullName);
}
