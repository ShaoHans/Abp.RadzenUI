using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Abp.RadzenUI.LocalizationTexts;

/// <summary>
/// A database-stored override (or addition) for a single localization text.
/// The static JSON files remain the baseline; only differences are stored here.
/// </summary>
public class LocalizationText : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// The localization resource name, e.g. "AbpRadzenUI", "AbpUi", the host resource name.
    /// </summary>
    public string ResourceName { get; set; } = default!;

    /// <summary>
    /// The culture name, e.g. "en", "zh-Hans".
    /// </summary>
    public string CultureName { get; set; } = default!;

    /// <summary>
    /// The localization key, e.g. "Menu:DataDictionary".
    /// </summary>
    public string Key { get; set; } = default!;

    /// <summary>
    /// The overridden text value.
    /// </summary>
    public string Value { get; set; } = default!;
}
