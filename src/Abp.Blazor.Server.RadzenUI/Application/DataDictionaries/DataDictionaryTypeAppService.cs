using System;
using Abp.RadzenUI.Application.Contracts.DataDictionaries;
using Abp.RadzenUI.DataDictionaries;
using Abp.RadzenUI.Localization;
using Abp.RadzenUI.Permissions;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

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

    public DataDictionaryTypeAppService(
        IRepository<DataDictionaryType, Guid> repository,
        IRepository<DataDictionaryItem, Guid> itemRepository,
        IDataDictionaryItemsCache itemsCache)
        : base(repository)
    {
        _itemRepository = itemRepository;
        _itemsCache = itemsCache;
        LocalizationResource = typeof(AbpRadzenUIResource);

        GetPolicyName = RadzenUIPermissions.DataDictionary.Default;
        GetListPolicyName = RadzenUIPermissions.DataDictionary.Default;
        CreatePolicyName = RadzenUIPermissions.DataDictionary.Create;
        UpdatePolicyName = RadzenUIPermissions.DataDictionary.Update;
        DeletePolicyName = RadzenUIPermissions.DataDictionary.Delete;
    }

    public override async Task<DataDictionaryTypeDto> CreateAsync(CreateDataDictionaryTypeDto input)
    {
        if (await Repository.AnyAsync(x => x.Code == input.Code))
        {
            throw new UserFriendlyException(L["DataDictionary:TypeCodeExist", input.Code]);
        }

        try
        {
            return await base.CreateAsync(input);
        }
        catch (Exception ex) when (IsDuplicateCodeException(ex))
        {
            throw new UserFriendlyException(L["DataDictionary:TypeCodeExist", input.Code]);
        }
    }

    public override async Task<DataDictionaryTypeDto> UpdateAsync(Guid id, UpdateDataDictionaryTypeDto input)
    {
        return await base.UpdateAsync(id, input);
    }

    public override async Task DeleteAsync(Guid id)
    {
        var entity = await Repository.GetAsync(id);
        var items = await _itemRepository.GetListAsync(x => x.DataDictionaryTypeId == id);

        if (items.Count != 0)
        {
            await _itemRepository.DeleteManyAsync(items, autoSave: true);
        }

        await base.DeleteAsync(id);
        await _itemsCache.RemoveByTypeCodeAsync(entity.Code);
    }

    protected override async Task<IQueryable<DataDictionaryType>> CreateFilteredQueryAsync(
        GetDataDictionaryTypesInput input)
    {
        var query = await base.CreateFilteredQueryAsync(input);

        if (!string.IsNullOrEmpty(input.Filter))
        {
            query = query.Where(x =>
                x.Code.Contains(input.Filter) ||
                x.Name.Contains(input.Filter));
        }

        return query;
    }

    private static bool IsDuplicateCodeException(Exception exception)
    {
        while (true)
        {
            if (exception.Message.Contains("IX_AbpDataDictionaryTypes", StringComparison.OrdinalIgnoreCase) ||
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
