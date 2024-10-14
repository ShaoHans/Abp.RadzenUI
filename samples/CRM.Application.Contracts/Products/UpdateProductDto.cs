using System.ComponentModel.DataAnnotations;
using CRM.Enums;

namespace CRM.Products;

public class UpdateProductDto
{
    [Required]
    [StringLength(ProductConsts.MaxNameLength)]
    public string Name { get; set; } = default!;

    [StringLength(ProductConsts.MaxImagePathLength)]
    public string? ImagePath { get; set; }

    public float Price { get; set; }

    public int StockCount { get; set; }

    public string? Description { get; set; }

    public ProductStatus Status { get; set; } = ProductStatus.OnSale;
}
