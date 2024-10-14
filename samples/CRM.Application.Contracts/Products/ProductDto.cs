using System;
using CRM.Enums;
using Volo.Abp.Application.Dtos;

namespace CRM.Products;

public class ProductDto : AuditedEntityDto<Guid>
{
    public string Code { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string? ImagePath { get; set; }

    public float Price { get; set; }

    public int StockCount { get; set; }

    public string? Description { get; set; }

    public ProductStatus Status { get; set; }
}
