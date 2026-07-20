using System.ComponentModel.DataAnnotations;

namespace Abp.RadzenUI.Application.Contracts.LocalizationTexts;

public class ResetLocalizationTextDto
{
    [Required]
    public string ResourceName { get; set; } = default!;

    [Required]
    public string CultureName { get; set; } = default!;

    [Required]
    public string Key { get; set; } = default!;
}
