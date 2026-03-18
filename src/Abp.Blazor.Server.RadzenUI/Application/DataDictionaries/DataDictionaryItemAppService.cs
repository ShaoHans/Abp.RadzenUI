using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.RadzenUI.Application.Contracts.DataDictionaries;
using Abp.RadzenUI.DataDictionaries;
using Abp.RadzenUI.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Repositories;

namespace Abp.RadzenUI.Application.DataDictionaries;

public class DataDictionaryItemAppService
    : CrudAppService<
        DataDictionaryItem,
        DataDictionaryItemDto,
        Guid,
        GetDataDictionaryItemsInput,
        CreateDataDictionaryItemDto,
        UpdateDataDictionaryItemDto
    >,
        IDataDictionaryItemAppService
{
    private readonly IRepository<DataDictionaryType, Guid> _typeRepository;
    private readonly IDistributedCache<List<DataDictionaryItemDto>> _cache;

    public DataDictionaryItemAppService(
        IRepository<DataDictionaryItem, Guid> repository,
        IRepository<DataDictionaryType, Guid> typeRepository,
        IDistributedCache<List<DataDictionaryItemDto>> cache)
        : base(repository)
    {
        _typeRepository = typeRepository;
        _cache = cache;

        GetPolicyName = RadzenUIPermissions.DataDictionary.Default;
        GetListPolicyName = RadzenUIPermissions.DataDictionary.Default;
        CreatePolicyName = RadzenUIPermissions.DataDictionary.Create;
        UpdatePolicyName = RadzenUIPermissions.DataDictionary.Update;
        DeletePolicyName = RadzenUIPermissions.DataDictionary.Delete;
    }

    public override async Task<DataDictionaryItemDto> CreateAsync(CreateDataDictionaryItemDto input)
    {
        if (await Repository.AnyAsync(x =>
            x.DataDictionaryTypeId == input.DataDictionaryTypeId && x.Code == input.Code))
        {
            throw new BusinessException(DataDictionaryErrorCodes.ItemCodeExist)
                .WithData("code", input.Code);
        }

        var result = await base.CreateAsync(input);
        await RemoveCacheAsync(input.DataDictionaryTypeId);
        return result;
    }

    public override async Task<DataDictionaryItemDto> UpdateAsync(Guid id, UpdateDataDictionaryItemDto input)
    {
        var entity = await Repository.GetAsync(id);
        var result = await base.UpdateAsync(id, input);
        await RemoveCacheAsync(entity.DataDictionaryTypeId);
        return result;
    }

    public override async Task DeleteAsync(Guid id)
    {
        var entity = await Repository.GetAsync(id);
        await base.DeleteAsync(id);
        await RemoveCacheAsync(entity.DataDictionaryTypeId);
    }

    [Authorize(RadzenUIPermissions.DataDictionary.Update)]
    public async Task ToggleActiveAsync(Guid id)
    {
        var entity = await Repository.GetAsync(id);
        entity.IsActive = !entity.IsActive;
        await Repository.UpdateAsync(entity);
        await RemoveCacheAsync(entity.DataDictionaryTypeId);
    }

    [AllowAnonymous]
    public async Task<List<DataDictionaryItemDto>> GetItemsByTypeCodeAsync(string typeCode)
    {
        var cacheKey = $"DataDictionary:{typeCode}";
        return await _cache.GetOrAddAsync(cacheKey, async () =>
        {
            var type = await _typeRepository.FindAsync(x => x.Code == typeCode);
            if (type == null)
            {
                return [];
            }

            var items = await Repository.GetListAsync(x =>
                x.DataDictionaryTypeId == type.Id && x.IsActive);

            return ObjectMapper.Map<List<DataDictionaryItem>, List<DataDictionaryItemDto>>(
                items.OrderBy(x => x.Sort).ToList());
        }) ?? [];
    }

    [AllowAnonymous]
    public async Task<DataDictionaryItemDto?> GetItemByTypeCodeAndItemCodeAsync(string typeCode, string itemCode)
    {
        var items = await GetItemsByTypeCodeAsync(typeCode);
        return items.Find(x => x.Code == itemCode);
    }

    protected override async Task<IQueryable<DataDictionaryItem>> CreateFilteredQueryAsync(
        GetDataDictionaryItemsInput input)
    {
        var query = await base.CreateFilteredQueryAsync(input);

        query = query.Where(x => x.DataDictionaryTypeId == input.DataDictionaryTypeId);

        if (!string.IsNullOrEmpty(input.Filter))
        {
            query = query.Where(x =>
                x.Code.Contains(input.Filter) ||
                x.Name.Contains(input.Filter));
        }

        return query;
    }

    private async Task RemoveCacheAsync(Guid typeId)
    {
        var type = await _typeRepository.FindAsync(typeId);
        if (type != null)
        {
            await _cache.RemoveAsync($"DataDictionary:{type.Code}");
        }
    }
}
