using CRM.Products;
using CRM.Operations;
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

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class WorkOrderToWorkOrderDtoMapper : MapperBase<WorkOrder, WorkOrderDto>
{
    public override partial WorkOrderDto Map(WorkOrder source);

    public override partial void Map(WorkOrder source, WorkOrderDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class WorkOrderEventToWorkOrderEventDtoMapper : MapperBase<WorkOrderEvent, WorkOrderEventDto>
{
    public override partial WorkOrderEventDto Map(WorkOrderEvent source);

    public override partial void Map(WorkOrderEvent source, WorkOrderEventDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class OperationAssetToOperationAssetDtoMapper : MapperBase<OperationAsset, OperationAssetDto>
{
    public override partial OperationAssetDto Map(OperationAsset source);

    public override partial void Map(OperationAsset source, OperationAssetDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class OperationShiftToOperationShiftDtoMapper : MapperBase<OperationShift, OperationShiftDto>
{
    [MapperIgnoreSource(nameof(OperationShift.CreationTime))]
    [MapperIgnoreSource(nameof(OperationShift.CreatorId))]
    [MapperIgnoreSource(nameof(OperationShift.LastModificationTime))]
    [MapperIgnoreSource(nameof(OperationShift.LastModifierId))]
    public override partial OperationShiftDto Map(OperationShift source);

    [MapperIgnoreSource(nameof(OperationShift.CreationTime))]
    [MapperIgnoreSource(nameof(OperationShift.CreatorId))]
    [MapperIgnoreSource(nameof(OperationShift.LastModificationTime))]
    [MapperIgnoreSource(nameof(OperationShift.LastModifierId))]
    public override partial void Map(OperationShift source, OperationShiftDto destination);
}
