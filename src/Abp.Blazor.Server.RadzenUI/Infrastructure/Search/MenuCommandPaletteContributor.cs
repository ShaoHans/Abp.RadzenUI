using Abp.RadzenUI.Infrastructure.Navigation;
using Abp.RadzenUI.Localization;
using Microsoft.Extensions.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.UI.Navigation;

namespace Abp.RadzenUI.Infrastructure.Search;

/// <summary>
/// Phase 1 contributor: turns the permission-filtered main menu into quick page jumps.
/// The menu returned by <see cref="IMenuManager"/> is already trimmed by ABP against the
/// current user's <c>RequirePermissions</c>, so no extra authorization is needed here.
/// </summary>
public class MenuCommandPaletteContributor(
    IMenuManager menuManager,
    IStringLocalizer<AbpRadzenUIResource> localizer)
    : ICommandPaletteContributor, ITransientDependency
{
    public string GroupKey => "CommandPalette:Group.Pages";

    public string GroupDisplayName => localizer["CommandPalette:Group.Pages"];

    public string? GroupIcon => "list_alt";

    public int Order => 0;

    public async Task<IReadOnlyList<CommandPaletteItem>> SearchAsync(
        CommandPaletteSearchContext context)
    {
        var keyword = context.Keyword.Trim();
        if (keyword.Length == 0)
        {
            return [];
        }

        var menu = await menuManager.GetMainMenuAsync();

        var results = new List<CommandPaletteItem>();
        foreach (var item in menu.Items)
        {
            CollectLeaves(item, [], results, keyword);
        }

        return results
            .OrderByDescending(static r => r.Score)
            .ThenBy(static r => r.Title, StringComparer.CurrentCultureIgnoreCase)
            .Take(context.MaxResultsPerGroup)
            .ToList();
    }

    static void CollectLeaves(
        ApplicationMenuItem item,
        IReadOnlyList<string> ancestors,
        List<CommandPaletteItem> results,
        string keyword)
    {
        var displayName = item.DisplayName ?? item.Name;

        // A navigable leaf (has a URL). Items with children act as breadcrumb ancestors.
        if (!string.IsNullOrWhiteSpace(item.Url))
        {
            var score = Score(displayName, ancestors, keyword);
            if (score > 0)
            {
                results.Add(new CommandPaletteItem
                {
                    Title = displayName,
                    Description = ancestors.Count > 0 ? string.Join(" › ", ancestors) : null,
                    Icon = string.IsNullOrWhiteSpace(item.Icon) ? "chevron_right" : item.Icon,
                    IconColor = item.GetIconColor(),
                    Url = item.Url!,
                    Score = score,
                });
            }
        }

        if (item.Items.IsNullOrEmpty())
        {
            return;
        }

        var childAncestors = new List<string>(ancestors) { displayName };
        foreach (var child in item.Items)
        {
            CollectLeaves(child, childAncestors, results, keyword);
        }
    }

    /// <summary>
    /// Match quality. 0 means no match. Title matches rank above breadcrumb matches;
    /// within a title, prefix &gt; word-start &gt; substring.
    /// </summary>
    static int Score(string title, IReadOnlyList<string> ancestors, string keyword)
    {
        const int prefix = 100;
        const int wordStart = 60;
        const int substring = 40;
        const int ancestorMatch = 10;

        if (title.StartsWith(keyword, StringComparison.CurrentCultureIgnoreCase))
        {
            return prefix;
        }

        var index = title.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase);
        if (index > 0 && char.IsWhiteSpace(title[index - 1]))
        {
            return wordStart;
        }

        if (index >= 0)
        {
            return substring;
        }

        foreach (var ancestor in ancestors)
        {
            if (ancestor.Contains(keyword, StringComparison.CurrentCultureIgnoreCase))
            {
                return ancestorMatch;
            }
        }

        return 0;
    }
}
