using Abp.RadzenUI;
using CRM.Localization;
using CRM.Permissions;
using CRM.Products;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace CRM.Blazor.Web.Components.Pages.Products;

public partial class List : IDisposable
{
    [Inject]
    public ISideDialogCoordinatorFactory SideDialogCoordinatorFactory { get; set; } = default!;

    private SideDialogCoordinator<ProductDto> _sideDialogCoordinator = default!;

    // Deep-link target used by the command palette (Ctrl+K) product search.
    [Parameter]
    [SupplyParameterFromQuery(Name = "code")]
    public string? CodeQuery { get; set; }

    private bool _initialFilterApplied;
    private string? _lastAppliedCode;

    public List()
    {
        ObjectMapperContext = typeof(AbpRadzenUIModule);
        LocalizationResource = typeof(CRMResource);

        CreatePolicyName = CRMPermissions.Products.Create;
        UpdatePolicyName = CRMPermissions.Products.Update;
        DeletePolicyName = CRMPermissions.Products.Delete;
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _sideDialogCoordinator = SideDialogCoordinatorFactory.Create<ProductDto>();
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        // Reload when navigated to a different ?code= while the page is already mounted.
        if (_grid is not null && !string.Equals(CodeQuery, _lastAppliedCode, StringComparison.Ordinal))
        {
            _initialFilterApplied = false;
            await _grid.Reload();
        }
    }

    protected override async Task UpdateGetListInputAsync(LoadDataArgs args)
    {
        if (!string.IsNullOrEmpty(args.Filter))
        {
            // A grid column filter takes precedence and clears the deep-link filter.
            GetListInput.Filter = args.Filter;
            _initialFilterApplied = true;
        }
        else if (!_initialFilterApplied && !string.IsNullOrWhiteSpace(CodeQuery))
        {
            // Escape " as "" for the Dynamic LINQ string literal.
            GetListInput.Filter = $"Code == \"{CodeQuery.Replace("\"", "\"\"")}\"";
            _initialFilterApplied = true;
            _lastAppliedCode = CodeQuery;
        }
        else
        {
            GetListInput.Filter = args.Filter;
        }

        await base.UpdateGetListInputAsync(args);
    }

    protected override Task<UpdateProductDto> SetEditDialogModelAsync(ProductDto dto)
    {
        return Task.FromResult(new UpdateProductDto
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            ImagePath = dto.ImagePath,
            Status = dto.Status,
            StockCount = dto.StockCount,
        });
    }

    private static DialogOptions SetDialogOptions()
    {
        return new DialogOptions
        {
            Draggable = true,
            Width = "600px",
        };
    }

    async Task OpenEditProductAsync(ProductDto product)
    {
        await OpenEditDialogAsync<Edit>(
            L["Edit"],
            product,
            SetDialogOptions,
            new Dictionary<string, object?> { { "Code", product.Code } }
        );
    }

    async Task OpenDetailProductAsync(ProductDto product)
    {
        await _sideDialogCoordinator.OpenDetailAsync<ProductDto, Detail>(
            product,
            product.Name,
            "Product",
            "520px"
        );
    }

    async Task DeleteProductAsync(ProductDto product)
    {
        await OpenDeleteConfirmDialogAsync(
            product.Id,
            L["Delete"],
            L["ProductDeletionConfirmationMessage", product.Name]
        );
    }

    public void Dispose()
    {
        _sideDialogCoordinator.Dispose();
    }
}
