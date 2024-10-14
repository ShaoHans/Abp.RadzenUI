using System;
using Volo.Abp.Application.Services;

namespace CRM.Products;

public interface IProductAppService
    : ICrudAppService<ProductDto, Guid, GetProductsInput, CreateProductDto, UpdateProductDto> { }
