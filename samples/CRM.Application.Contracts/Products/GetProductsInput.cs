using CRM.Enums;
using Volo.Abp.Application.Dtos;

namespace CRM.Products;

public class GetProductsInput : ExtensiblePagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }

    public ProductStatus? Status { get; set; }
}
