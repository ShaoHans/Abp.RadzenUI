using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace CRM.Products;

public interface IProductAppService
    : ICrudAppService<ProductDto, Guid, GetProductsInput, CreateProductDto, UpdateProductDto>
{
    /// <summary>
    /// Lightweight lookup used by the command palette: matches products whose code or
    /// name contains <paramref name="keyword"/>.
    /// </summary>
    Task<List<ProductDto>> SearchAsync(string keyword, int maxResultCount = 8);
}
