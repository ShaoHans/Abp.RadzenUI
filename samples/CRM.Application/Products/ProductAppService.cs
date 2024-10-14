using System;
using System.Linq;
using System.Threading.Tasks;
using CRM.Permissions;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace CRM.Products;

public class ProductAppService
    : CrudAppService<
        Product,
        ProductDto,
        Guid,
        GetProductsInput,
        CreateProductDto,
        UpdateProductDto
    >,
        IProductAppService
{
    public ProductAppService(IRepository<Product, Guid> repository)
        : base(repository)
    {
        GetPolicyName = CRMPermissions.Products.Default;
        GetListPolicyName = CRMPermissions.Products.Default;
        CreatePolicyName = CRMPermissions.Products.Create;
        UpdatePolicyName = CRMPermissions.Products.Update;
        DeletePolicyName = CRMPermissions.Products.Delete;
    }

    public override async Task<ProductDto> CreateAsync(CreateProductDto input)
    {
        if (await Repository.CountAsync(x => x.Code == input.Code) > 0)
        {
            throw new BusinessException(CRMDomainErrorCodes.ProductCodeExist).WithData(
                "productCode",
                input.Code
            );
        }

        return await base.CreateAsync(input);
    }

    protected override async Task<IQueryable<Product>> CreateFilteredQueryAsync(
        GetProductsInput input
    )
    {
        var query = await base.CreateFilteredQueryAsync(input);

        query = query
            .WhereIf(!string.IsNullOrEmpty(input.Filter), x => x.Name.Contains(input.Filter!))
            .WhereIf(input.Status.HasValue, x => x.Status == input.Status);

        return query;
    }
}
