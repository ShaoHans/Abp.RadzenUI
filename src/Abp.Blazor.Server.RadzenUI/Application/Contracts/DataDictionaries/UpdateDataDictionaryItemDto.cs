using System.ComponentModel.DataAnnotations;
using Abp.RadzenUI.DataDictionaries;

namespace Abp.RadzenUI.Application.Contracts.DataDictionaries;

public class UpdateDataDictionaryItemDto
{
    [Required]
    [StringLength(DataDictionaryItemConsts.MaxNameLength)]
    public string Name { get; set; } = default!;

    [Required]
    public int Sort { get; set; }

    public bool IsActive { get; set; }

    [StringLength(DataDictionaryItemConsts.MaxDescriptionLength)]
    public string? Description { get; set; }
}
