using CRM.Products;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace CRM;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ProductToProductDtoMapper : MapperBase<Product, ProductDto>
{
    public override partial ProductDto Map(Product source);

    public override partial void Map(Product source, ProductDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateProductDtoToProductMapper : MapperBase<CreateProductDto, Product>
{
    [MapperIgnoreTarget(nameof(Product.CreatorId))]
    [MapperIgnoreTarget(nameof(Product.CreationTime))]
    [MapperIgnoreTarget(nameof(Product.LastModifierId))]
    [MapperIgnoreTarget(nameof(Product.LastModificationTime))]
    [MapperIgnoreTarget(nameof(Product.ConcurrencyStamp))]
    public override partial Product Map(CreateProductDto source);

    [MapperIgnoreTarget(nameof(Product.CreatorId))]
    [MapperIgnoreTarget(nameof(Product.CreationTime))]
    [MapperIgnoreTarget(nameof(Product.LastModifierId))]
    [MapperIgnoreTarget(nameof(Product.LastModificationTime))]
    [MapperIgnoreTarget(nameof(Product.ConcurrencyStamp))]
    public override partial void Map(CreateProductDto source, Product destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class UpdateProductDtoToProductMapper : MapperBase<UpdateProductDto, Product>
{
    [MapperIgnoreTarget(nameof(Product.CreatorId))]
    [MapperIgnoreTarget(nameof(Product.CreationTime))]
    [MapperIgnoreTarget(nameof(Product.LastModifierId))]
    [MapperIgnoreTarget(nameof(Product.LastModificationTime))]
    [MapperIgnoreTarget(nameof(Product.ConcurrencyStamp))]
    [MapperIgnoreTarget(nameof(Product.Code))]
    public override partial Product Map(UpdateProductDto source);

    [MapperIgnoreTarget(nameof(Product.CreatorId))]
    [MapperIgnoreTarget(nameof(Product.CreationTime))]
    [MapperIgnoreTarget(nameof(Product.LastModifierId))]
    [MapperIgnoreTarget(nameof(Product.LastModificationTime))]
    [MapperIgnoreTarget(nameof(Product.ConcurrencyStamp))]
    [MapperIgnoreTarget(nameof(Product.Code))]
    public override partial void Map(UpdateProductDto source, Product destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ProductDtoToUpdateProductDtoMapper : MapperBase<ProductDto, UpdateProductDto>
{
    public override partial UpdateProductDto Map(ProductDto source);

    public override partial void Map(ProductDto source, UpdateProductDto destination);
}
