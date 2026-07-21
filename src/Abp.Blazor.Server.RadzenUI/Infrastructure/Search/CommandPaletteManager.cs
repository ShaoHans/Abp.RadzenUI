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

    public async Task<IReadOnlyList<CommandPaletteResultGroup>> SearchAsync(
        string keyword,
        CancellationToken cancellationToken = default)
    {
        keyword = keyword?.Trim() ?? string.Empty;

        if (!Enabled || keyword.Length < _options.MinKeywordLength)
        {
            return [];
        }

        var context = new CommandPaletteSearchContext
        {
            Keyword = keyword,
            MaxResultsPerGroup = _options.MaxResultsPerGroup,
            CancellationToken = cancellationToken,
        };

        var groups = new Dictionary<string, (string DisplayName, int Order, List<CommandPaletteItem> Items)>();

        foreach (var contributorType in _options.Contributors)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (serviceProvider.GetService(contributorType) is not ICommandPaletteContributor contributor)
            {
                logger.LogWarning(
                    "Command palette contributor {Type} is not registered in DI and was skipped.",
                    contributorType.FullName);
                continue;
            }

            try
            {
                var items = await contributor.SearchAsync(context);
                if (items.Count == 0)
                {
                    continue;
                }

                if (!groups.TryGetValue(contributor.GroupKey, out var bucket))
                {
                    bucket = (contributor.GroupDisplayName, contributor.Order, []);
                    groups[contributor.GroupKey] = bucket;
                }

                bucket.Items.AddRange(items);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                // A single failing contributor must not break the whole palette.
                logger.LogError(ex,
                    "Command palette contributor {Type} threw and was skipped.",
                    contributorType.FullName);
            }
        }

        return groups
            .Select(kvp => new CommandPaletteResultGroup
            {
                GroupKey = kvp.Key,
                DisplayName = kvp.Value.DisplayName,
                Order = kvp.Value.Order,
                Items = kvp.Value.Items
                    .OrderByDescending(static i => i.Score)
                    .Take(_options.MaxResultsPerGroup)
                    .ToList(),
            })
            .Where(static g => g.Items.Count > 0)
            .OrderBy(static g => g.Order)
            .ToList();
    }
}
