using System;
using System.Linq;
using System.Linq.Dynamic.Core;
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

        /*
         the input.Filter is a string that contains a dynamic expression to filter the query.
         For example: "Name.Contains('abc') && Price > 100"
         We can use the Dynamic LINQ library to apply the filter to the query.
         You need install the nuget package on your XXX.EntityFrameworkCore project: Microsoft.EntityFrameworkCore.DynamicLinq
         and then use the namespace : using System.Linq.Dynamic.Core;
         */
        if (!string.IsNullOrEmpty(input.Filter))
        {
            query = query.Where(input.Filter);
        }

        return query;
    }
}
