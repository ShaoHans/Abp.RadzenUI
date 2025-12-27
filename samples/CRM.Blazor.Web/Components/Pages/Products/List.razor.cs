using Abp.RadzenUI;
using CRM.Localization;
using CRM.Permissions;
using CRM.Products;
using Radzen;

namespace CRM.Blazor.Web.Components.Pages.Products;

public partial class List
{
    public List()
    {
        ObjectMapperContext = typeof(AbpRadzenUIModule);
        LocalizationResource = typeof(CRMResource);

        CreatePolicyName = CRMPermissions.Products.Create;
        UpdatePolicyName = CRMPermissions.Products.Update;
        DeletePolicyName = CRMPermissions.Products.Delete;
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
}
