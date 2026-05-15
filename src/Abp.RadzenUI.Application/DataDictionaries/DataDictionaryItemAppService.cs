using Abp.RadzenUI.Application.Contracts.DataDictionaries;
using Abp.RadzenUI.DataDictionaries;
using Abp.RadzenUI.Localization;
using Abp.RadzenUI.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;

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
    private readonly IDataFilter _dataFilter;

    public DataDictionaryItemAppService(
        IRepository<DataDictionaryItem, Guid> repository,
        IRepository<DataDictionaryType, Guid> typeRepository,
        IDataDictionaryItemsCache itemsCache,
        IDataFilter dataFilter
    )
        : base(repository)
    {
        _typeRepository = typeRepository;
        _itemsCache = itemsCache;
        _dataFilter = dataFilter;
        LocalizationResource = typeof(AbpRadzenUIResource);

        GetPolicyName = RadzenUIPermissions.DataDictionary.Default;
        GetListPolicyName = RadzenUIPermissions.DataDictionary.Default;
        CreatePolicyName = RadzenUIPermissions.DataDictionary.Create;
        UpdatePolicyName = RadzenUIPermissions.DataDictionary.Update;
        DeletePolicyName = RadzenUIPermissions.DataDictionary.Delete;
    }

    public override async Task<DataDictionaryItemDto> CreateAsync(CreateDataDictionaryItemDto input)
    {
        input.Code = input.Code.Trim();

        var type = await GetAccessibleTypeByIdAsync(input.DataDictionaryTypeId);
        EnsureTypeCanBeManaged(type);

        if (
            await Repository.AnyAsync(x =>
                x.DataDictionaryTypeId == input.DataDictionaryTypeId && x.Code == input.Code
            )
        )
        {
            throw new BusinessException(DataDictionaryErrorCodes.ItemCodeExist).WithData(
                "code",
                input.Code
            );
        }

        var result = await base.CreateAsync(input);
        await _itemsCache.RemoveByTypeIdAsync(input.DataDictionaryTypeId);
        return result;
    }

    public override async Task<DataDictionaryItemDto> UpdateAsync(
        Guid id,
        UpdateDataDictionaryItemDto input
    )
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
        var type = await FindAccessibleTypeByCodeAsync(typeCode);
        if (type == null)
        {
            return [];
        }

        return await _itemsCache.GetOrAddByTypeAsync(
            type,
            async () =>
            {
                var items = await GetActiveItemsAsync(type);

                return ObjectMapper.Map<List<DataDictionaryItem>, List<DataDictionaryItemDto>>(
                    items.OrderBy(x => x.Sort).ToList()
                );
            }
        );
    }

    [AllowAnonymous]
    public async Task<DataDictionaryItemDto?> GetItemByTypeCodeAndItemCodeAsync(
        string typeCode,
        string itemCode
    )
    {
        var items = await GetItemsByTypeCodeAsync(typeCode);
        return items.Find(x => x.Code == itemCode);
    }

    public override async Task<PagedResultDto<DataDictionaryItemDto>> GetListAsync(
        GetDataDictionaryItemsInput input
    )
    {
        await CheckGetListPolicyAsync();

        var type = await GetAccessibleTypeByIdAsync(input.DataDictionaryTypeId);

        if (!(type.TenantId == null && type.IsShared && CurrentTenant.IsAvailable))
        {
            return await base.GetListAsync(input);
        }

        using (_dataFilter.Disable<IMultiTenant>())
        {
            var query = (await Repository.GetQueryableAsync()).Where(x =>
                x.TenantId == null && x.DataDictionaryTypeId == input.DataDictionaryTypeId
            );

            if (!string.IsNullOrEmpty(input.Filter))
            {
                query = query.Where(x =>
                    x.Code.Contains(input.Filter) || x.Name.Contains(input.Filter)
                );
            }

            var totalCount = await AsyncExecuter.CountAsync(query);
            query = ApplySorting(query, input);
            query = ApplyPaging(query, input);

            var entities = await AsyncExecuter.ToListAsync(query);

            return new PagedResultDto<DataDictionaryItemDto>(
                totalCount,
                await MapToGetListOutputDtosAsync(entities)
            );
        }
    }

    protected override async Task<IQueryable<DataDictionaryItem>> CreateFilteredQueryAsync(
        GetDataDictionaryItemsInput input
    )
    {
        var type = await GetAccessibleTypeByIdAsync(input.DataDictionaryTypeId);
        IQueryable<DataDictionaryItem> query = type.TenantId == null && type.IsShared && CurrentTenant.IsAvailable
            ? (await Repository.GetQueryableAsync()).Where(x => x.DataDictionaryTypeId == input.DataDictionaryTypeId)
            : await base.CreateFilteredQueryAsync(input);

        query = query.Where(x => x.DataDictionaryTypeId == input.DataDictionaryTypeId);

        if (!string.IsNullOrEmpty(input.Filter))
        {
            query = query.Where(x =>
                x.Code.Contains(input.Filter) || x.Name.Contains(input.Filter)
            );
        }

        return query;
    }

    protected override IQueryable<DataDictionaryItem> ApplyDefaultSorting(
        IQueryable<DataDictionaryItem> query
    )
    {
        return query.OrderBy(x => x.Sort);
    }

    private static bool IsDuplicateCodeException(Exception exception)
    {
        while (true)
        {
            if (
                exception.Message.Contains(
                    "IX_AbpDataDictionaryItems",
                    StringComparison.OrdinalIgnoreCase
                )
                || exception.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase)
                || exception.Message.Contains("unique", StringComparison.OrdinalIgnoreCase)
            )
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

    private async Task<DataDictionaryType> GetAccessibleTypeByIdAsync(Guid typeId)
    {
        if (!CurrentTenant.IsAvailable)
        {
            return await _typeRepository.GetAsync(typeId);
        }

        using (_dataFilter.Disable<IMultiTenant>())
        {
            var type = await _typeRepository.GetAsync(typeId);

            if (type.TenantId == CurrentTenant.Id || (type.TenantId == null && type.IsShared))
            {
                return type;
            }
        }

        throw new BusinessException(DataDictionaryErrorCodes.SharedTypeIsReadOnlyForTenant);
    }

    private async Task<DataDictionaryType?> FindAccessibleTypeByCodeAsync(string typeCode)
    {
        if (!CurrentTenant.IsAvailable)
        {
            return await _typeRepository.FindAsync(x => x.Code == typeCode);
        }

        using (_dataFilter.Disable<IMultiTenant>())
        {
            var types = await _typeRepository.GetListAsync(x =>
                x.Code == typeCode
                && (x.TenantId == CurrentTenant.Id || (x.TenantId == null && x.IsShared))
            );

            return types.FirstOrDefault(x => x.TenantId == CurrentTenant.Id)
                ?? types.FirstOrDefault(x => x.TenantId == null && x.IsShared);
        }
    }

    private async Task<List<DataDictionaryItem>> GetActiveItemsAsync(DataDictionaryType type)
    {
        if (type.TenantId == null && type.IsShared && CurrentTenant.IsAvailable)
        {
            using (_dataFilter.Disable<IMultiTenant>())
            {
                return await Repository.GetListAsync(x =>
                    x.TenantId == null && x.DataDictionaryTypeId == type.Id && x.IsActive
                );
            }
        }

        return await Repository.GetListAsync(x => x.DataDictionaryTypeId == type.Id && x.IsActive);
    }

    private void EnsureTypeCanBeManaged(DataDictionaryType type)
    {
        if (CurrentTenant.IsAvailable && type.TenantId == null && type.IsShared)
        {
            throw new BusinessException(DataDictionaryErrorCodes.SharedTypeIsReadOnlyForTenant);
        }
    }
}
