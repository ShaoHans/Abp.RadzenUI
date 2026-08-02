using System.Collections.Generic;

namespace Abp.RadzenUI.Application.LocalizationTexts;

/// <summary>
/// Distributed-cache payload holding the database override texts for a single
/// (tenant, resource, culture) tuple.
/// </summary>
public class LocalizationTextCacheItem
{
    public Dictionary<string, string> Texts { get; set; } = [];
}
