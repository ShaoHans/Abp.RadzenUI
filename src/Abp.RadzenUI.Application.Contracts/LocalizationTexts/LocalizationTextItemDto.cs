using System;

namespace Abp.RadzenUI.Application.Contracts.LocalizationTexts;

public class LocalizationTextItemDto
{
    /// <summary>
    /// The database override id, if this key is currently overridden.
    /// </summary>
    public Guid? Id { get; set; }

    public string ResourceName { get; set; } = default!;

    public string CultureName { get; set; } = default!;

    public string Key { get; set; } = default!;

    /// <summary>
    /// The static (JSON) baseline value. Null when the key only exists as a database override.
    /// </summary>
    public string? BaseValue { get; set; }

    /// <summary>
    /// The database override value. Null when the key is not overridden.
    /// </summary>
    public string? OverrideValue { get; set; }

    public bool IsOverridden => Id.HasValue;
}
