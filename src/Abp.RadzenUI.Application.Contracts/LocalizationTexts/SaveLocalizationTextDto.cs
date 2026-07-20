using System.ComponentModel.DataAnnotations;
using Abp.RadzenUI.LocalizationTexts;

namespace Abp.RadzenUI.Application.Contracts.LocalizationTexts;

public class SaveLocalizationTextDto
{
    [Required]
    [StringLength(LocalizationTextConsts.MaxResourceNameLength)]
    public string ResourceName { get; set; } = default!;

    [Required]
    [StringLength(LocalizationTextConsts.MaxCultureNameLength)]
    public string CultureName { get; set; } = default!;

    [Required]
    [StringLength(LocalizationTextConsts.MaxKeyLength)]
    public string Key { get; set; } = default!;

    [Required]
    [StringLength(LocalizationTextConsts.MaxValueLength)]
    public string Value { get; set; } = default!;
}
