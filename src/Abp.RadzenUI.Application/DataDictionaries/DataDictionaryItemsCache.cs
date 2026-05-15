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
    public async Task<List<DataDictionaryItemDto>> GetOrAddByTypeAsync(
        DataDictionaryType type,
        Func<Task<List<DataDictionaryItemDto>>> factory)
    {
        Check.NotNull(type, nameof(type));
        Check.NotNull(factory, nameof(factory));

        return await cache.GetOrAddAsync(GetCacheKey(type), factory) ?? [];
    }

    public async Task RemoveByTypeIdAsync(Guid typeId)
    {
        var type = await typeRepository.FindAsync(typeId);
        if (type == null)
        {
            return;
        }

        await cache.RemoveAsync(GetCacheKey(type));
    }

    private static string GetCacheKey(DataDictionaryType type)
    {
        return $"DataDictionary:{type.TenantId?.ToString("D") ?? "host"}:{type.Code}";
    }
}