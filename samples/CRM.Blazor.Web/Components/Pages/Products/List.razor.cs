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
    public SideDialogState<ProductDto> ProductDialogState { get; set; } = default!;

    private SideDialogCoordinator<ProductDto> _sideDialogCoordinator = default!;

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
        _sideDialogCoordinator = new SideDialogCoordinator<ProductDto>(DialogService, ProductDialogState);
        _sideDialogCoordinator.Attach();
    }

    protected override async Task UpdateGetListInputAsync(LoadDataArgs args)
    {
        GetListInput.Filter = args.Filter;
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
        _sideDialogCoordinator.Detach();
    }
}
