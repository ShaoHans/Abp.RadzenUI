using Abp.RadzenUI.Application.Contracts.DataDictionaries;
using Abp.RadzenUI.DataDictionaries;
using Abp.RadzenUI.Localization;
using Abp.RadzenUI.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;

namespace Abp.RadzenUI.Application.DataDictionaries;

public class DataDictionaryTypeAppService
    : CrudAppService<
        DataDictionaryType,
        DataDictionaryTypeDto,
        Guid,
        GetDataDictionaryTypesInput,
        CreateDataDictionaryTypeDto,
        UpdateDataDictionaryTypeDto
    >,
        IDataDictionaryTypeAppService
{
    private readonly IRepository<DataDictionaryItem, Guid> _itemRepository;
    private readonly IDataDictionaryItemsCache _itemsCache;
    private readonly IDataFilter _dataFilter;

    public DataDictionaryTypeAppService(
        IRepository<DataDictionaryType, Guid> repository,
        IRepository<DataDictionaryItem, Guid> itemRepository,
        IDataDictionaryItemsCache itemsCache,
        IDataFilter dataFilter
    )
        : base(repository)
    {
        _itemRepository = itemRepository;
        _itemsCache = itemsCache;
        _dataFilter = dataFilter;
        LocalizationResource = typeof(AbpRadzenUIResource);

        GetPolicyName = RadzenUIPermissions.DataDictionary.Default;
        GetListPolicyName = RadzenUIPermissions.DataDictionary.Default;
        CreatePolicyName = RadzenUIPermissions.DataDictionary.Create;
        UpdatePolicyName = RadzenUIPermissions.DataDictionary.Update;
        DeletePolicyName = RadzenUIPermissions.DataDictionary.Delete;
    }

    public override async Task<DataDictionaryTypeDto> CreateAsync(CreateDataDictionaryTypeDto input)
    {
        input.Code = input.Code.Trim();
        EnsureSharedTypeCanBeManaged(input.IsShared);

        if (await Repository.AnyAsync(x => x.Code == input.Code))
        {
            throw new BusinessException(DataDictionaryErrorCodes.TypeCodeExist).WithData(
                "code",
                input.Code
            );
        }

        return await base.CreateAsync(input);
    }

    public override async Task<DataDictionaryTypeDto> UpdateAsync(
        Guid id,
        UpdateDataDictionaryTypeDto input
    )
    {
        EnsureSharedTypeCanBeManaged(input.IsShared);
        return await base.UpdateAsync(id, input);
    }

    public override async Task DeleteAsync(Guid id)
    {
        var entity = await Repository.GetAsync(id);
        var items = await _itemRepository.GetListAsync(x => x.DataDictionaryTypeId == id);

        EnsureTypeCanBeManaged(entity);

        await _itemsCache.RemoveByTypeIdAsync(id);

        if (items.Count != 0)
        {
            await _itemRepository.DeleteManyAsync(items, autoSave: true);
        }

        await base.DeleteAsync(id);
    }

    public override async Task<PagedResultDto<DataDictionaryTypeDto>> GetListAsync(
        GetDataDictionaryTypesInput input
    )
    {
        await CheckGetListPolicyAsync();

        if (!CurrentTenant.IsAvailable)
        {
            return await base.GetListAsync(input);
        }

        using (_dataFilter.Disable<IMultiTenant>())
        {
            var query = (await Repository.GetQueryableAsync()).Where(x =>
                x.TenantId == CurrentTenant.Id || (x.TenantId == null && x.IsShared)
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

            return new PagedResultDto<DataDictionaryTypeDto>(
                totalCount,
                await MapToGetListOutputDtosAsync(entities)
            );
        }
    }

    protected override async Task<IQueryable<DataDictionaryType>> CreateFilteredQueryAsync(
        GetDataDictionaryTypesInput input
    )
    {
        var query = await Repository.GetQueryableAsync();

        if (!string.IsNullOrEmpty(input.Filter))
        {
            query = query.Where(x =>
                x.Code.Contains(input.Filter) || x.Name.Contains(input.Filter)
            );
        }

        return query;
    }

    private void EnsureSharedTypeCanBeManaged(bool isShared)
    {
        if (isShared && CurrentTenant.IsAvailable)
        {
            throw new BusinessException(DataDictionaryErrorCodes.SharedTypeCanOnlyBeManagedByHost);
        }
    }

    private void EnsureTypeCanBeManaged(DataDictionaryType type)
    {
        if (CurrentTenant.IsAvailable && type.TenantId == null && type.IsShared)
        {
            throw new BusinessException(DataDictionaryErrorCodes.SharedTypeIsReadOnlyForTenant);
        }
    }
}
