using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.RadzenUI.Infrastructure.Search;
using CRM.Localization;
using CRM.Permissions;
using CRM.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Volo.Abp.DependencyInjection;

namespace CRM.Blazor.Web.Search;

/// <summary>
/// Phase 2 example: extends the command palette (Ctrl+K) with product search by code or
/// name. Registered via <see cref="CommandPaletteOptions"/> in the web module — the palette
/// UI needs no changes. Each result deep-links to the product list filtered by code.
/// </summary>
public class ProductCommandPaletteContributor(
    IProductAppService productAppService,
    IAuthorizationService authorizationService,
    IStringLocalizer<CRMResource> l)
    : ICommandPaletteContributor, ITransientDependency
{
    public string GroupKey => "CommandPalette:Group.Products";

    public string GroupDisplayName => l["CommandPalette:Group.Products"];

    public int Order => 10;

    public async Task<IReadOnlyList<CommandPaletteItem>> SearchAsync(
        CommandPaletteSearchContext context)
    {
        // Skip quietly (no results, no error log) when the user can't view products.
        if (!await authorizationService.IsGrantedAsync(CRMPermissions.Products.Default))
        {
            return [];
        }

        var products = await productAppService.SearchAsync(
            context.Keyword,
            context.MaxResultsPerGroup);

        return products
            .Select(p => new CommandPaletteItem
            {
                Title = p.Name,
                Description = $"{l["DisplayName:Code"]}: {p.Code}",
                Icon = "inventory_2",
                IconColor = "#ea580c",
                Url = $"/products?code={Uri.EscapeDataString(p.Code)}",
                Score = ScoreOf(p, context.Keyword),
            })
            .ToList();
    }

    static int ScoreOf(ProductDto product, string keyword)
    {
        if (string.Equals(product.Code, keyword, StringComparison.OrdinalIgnoreCase))
        {
            return 100;
        }

        if (product.Code.StartsWith(keyword, StringComparison.OrdinalIgnoreCase))
        {
            return 80;
        }

        if (product.Name.StartsWith(keyword, StringComparison.CurrentCultureIgnoreCase))
        {
            return 70;
        }

        return product.Name.Contains(keyword, StringComparison.CurrentCultureIgnoreCase)
            ? 50
            : 40;
    }
}
