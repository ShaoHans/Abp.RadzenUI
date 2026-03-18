using System;
using System.ComponentModel.DataAnnotations;
using Abp.RadzenUI.DataDictionaries;

namespace Abp.RadzenUI.Application.Contracts.DataDictionaries;

public class CreateDataDictionaryItemDto
{
    [Required]
    public Guid DataDictionaryTypeId { get; set; }

    [Required]
    [StringLength(DataDictionaryItemConsts.MaxCodeLength)]
    public string Code { get; set; } = default!;

    [Required]
    [StringLength(DataDictionaryItemConsts.MaxNameLength)]
    public string Name { get; set; } = default!;

    [Required]
    public int Sort { get; set; }

    [StringLength(DataDictionaryItemConsts.MaxDescriptionLength)]
    public string? Description { get; set; }
}
