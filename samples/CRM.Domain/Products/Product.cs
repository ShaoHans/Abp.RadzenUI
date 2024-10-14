using System;
using CRM.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace CRM.Products;

public class Product : AuditedAggregateRoot<Guid>
{
    public string Code { get; set; } = default!;

    public string Name { get; set; } = default!;

    public float Price { get; set; }

    public int StockCount { get; set; }

    public string? ImagePath { get; set; }

    public string? Description { get; set; }

    public ProductStatus Status { get; set; }
}
