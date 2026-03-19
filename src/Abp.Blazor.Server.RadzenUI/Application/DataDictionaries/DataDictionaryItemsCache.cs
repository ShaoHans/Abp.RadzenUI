using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.RadzenUI.Application.Contracts.DataDictionaries;
using Abp.RadzenUI.DataDictionaries;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Repositories;

namespace Abp.RadzenUI.Application.DataDictionaries;

public class DataDictionaryItemsCache(
    IDistributedCache<List<DataDictionaryItemDto>> cache,
    IRepository<DataDictionaryType, Guid> typeRepository)
    : IDataDictionaryItemsCache
{
    public async Task<List<DataDictionaryItemDto>> GetOrAddByTypeCodeAsync(
        string typeCode,
        Func<Task<List<DataDictionaryItemDto>>> factory)
    {
        Check.NotNullOrWhiteSpace(typeCode, nameof(typeCode));
        Check.NotNull(factory, nameof(factory));

        return await cache.GetOrAddAsync(GetCacheKey(typeCode), factory) ?? [];
    }

    public async Task RemoveByTypeCodeAsync(string typeCode)
    {
        if (string.IsNullOrWhiteSpace(typeCode))
        {
            return;
        }

        await cache.RemoveAsync(GetCacheKey(typeCode));
    }

    public async Task RemoveByTypeIdAsync(Guid typeId)
    {
        var type = await typeRepository.FindAsync(typeId);
        if (type == null)
        {
            return;
        }

        await RemoveByTypeCodeAsync(type.Code);
    }

    private static string GetCacheKey(string typeCode)
    {
        return $"DataDictionary:{typeCode}";
    }
}