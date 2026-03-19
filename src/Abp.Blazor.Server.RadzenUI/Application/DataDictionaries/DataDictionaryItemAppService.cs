using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.RadzenUI.Application.Contracts.DataDictionaries;
using Abp.RadzenUI.DataDictionaries;
using Abp.RadzenUI.Localization;
using Abp.RadzenUI.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;
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
    private readonly IDataDictionaryItemsCache _itemsCache;

    public DataDictionaryItemAppService(
        IRepository<DataDictionaryItem, Guid> repository,
        IRepository<DataDictionaryType, Guid> typeRepository,
        IDataDictionaryItemsCache itemsCache)
        : base(repository)
    {
        _typeRepository = typeRepository;
        _itemsCache = itemsCache;
        LocalizationResource = typeof(AbpRadzenUIResource);

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
            throw new UserFriendlyException(L["DataDictionary:ItemCodeExist", input.Code]);
        }

        DataDictionaryItemDto result;

        try
        {
            result = await base.CreateAsync(input);
        }
        catch (Exception ex) when (IsDuplicateCodeException(ex))
        {
            throw new UserFriendlyException(L["DataDictionary:ItemCodeExist", input.Code]);
        }

        await _itemsCache.RemoveByTypeIdAsync(input.DataDictionaryTypeId);
        return result;
    }

    public override async Task<DataDictionaryItemDto> UpdateAsync(Guid id, UpdateDataDictionaryItemDto input)
    {
        var entity = await Repository.GetAsync(id);

        var result = await base.UpdateAsync(id, input);
        await _itemsCache.RemoveByTypeIdAsync(entity.DataDictionaryTypeId);
        return result;
    }

    public override async Task DeleteAsync(Guid id)
    {
        var entity = await Repository.GetAsync(id);
        await base.DeleteAsync(id);
        await _itemsCache.RemoveByTypeIdAsync(entity.DataDictionaryTypeId);
    }

    [Authorize(RadzenUIPermissions.DataDictionary.Update)]
    public async Task ToggleActiveAsync(Guid id)
    {
        var entity = await Repository.GetAsync(id);
        entity.IsActive = !entity.IsActive;
        await Repository.UpdateAsync(entity);
        await _itemsCache.RemoveByTypeIdAsync(entity.DataDictionaryTypeId);
    }

    [AllowAnonymous]
    public async Task<List<DataDictionaryItemDto>> GetItemsByTypeCodeAsync(string typeCode)
    {
        return await _itemsCache.GetOrAddByTypeCodeAsync(typeCode, async () =>
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
        });
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

    protected override IQueryable<DataDictionaryItem> ApplyDefaultSorting(IQueryable<DataDictionaryItem> query)
    {
        return query.OrderBy(x => x.Sort);
    }

    private static bool IsDuplicateCodeException(Exception exception)
    {
        while (true)
        {
            if (exception.Message.Contains("IX_AbpDataDictionaryItems", StringComparison.OrdinalIgnoreCase) ||
                exception.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) ||
                exception.Message.Contains("unique", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (exception.InnerException == null)
            {
                return false;
            }

            exception = exception.InnerException;
        }
    }
}
