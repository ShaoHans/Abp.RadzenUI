using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Abp.RadzenUI.Application.Contracts.LocalizationTexts;

public class GetLocalizationTextsInput : PagedAndSortedResultRequestDto
{
    [Required]
    public string ResourceName { get; set; } = default!;

    [Required]
    public string CultureName { get; set; } = default!;

    /// <summary>
    /// Filters by key or value (contains, case-insensitive).
    /// </summary>
    public string? Filter { get; set; }

    /// <summary>
    /// When true, only returns keys that currently have a database override.
    /// </summary>
    public bool OnlyOverridden { get; set; }
}
