using System.ComponentModel.DataAnnotations;
using Abp.RadzenUI.DataDictionaries;

namespace Abp.RadzenUI.Application.Contracts.DataDictionaries;

public class UpdateDataDictionaryTypeDto
{
    public bool IsShared { get; set; }

    [Required]
    [StringLength(DataDictionaryTypeConsts.MaxNameLength)]
    public string Name { get; set; } = default!;

    [StringLength(DataDictionaryTypeConsts.MaxDescriptionLength)]
    public string? Description { get; set; }
}
