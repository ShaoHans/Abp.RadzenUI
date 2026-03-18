using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.RadzenUI.Application.Contracts.DataDictionaries;
using Abp.RadzenUI.DataDictionaries;
using Abp.RadzenUI.Permissions;
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
    public DataDictionaryTypeAppService(IRepository<DataDictionaryType, Guid> repository)
        : base(repository)
    {
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
            throw new BusinessException(DataDictionaryErrorCodes.TypeCodeExist)
                .WithData("code", input.Code);
        }

        return await base.CreateAsync(input);
    }

    public override async Task DeleteAsync(Guid id)
    {
        var itemRepository = LazyServiceProvider.LazyGetRequiredService<IRepository<DataDictionaryItem, Guid>>();
        if (await itemRepository.AnyAsync(x => x.DataDictionaryTypeId == id))
        {
            throw new BusinessException(DataDictionaryErrorCodes.TypeHasItems);
        }

        await base.DeleteAsync(id);
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
}
