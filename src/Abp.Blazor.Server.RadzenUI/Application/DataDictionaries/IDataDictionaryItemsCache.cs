using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.RadzenUI.Application.Contracts.DataDictionaries;
using Volo.Abp.DependencyInjection;

namespace Abp.RadzenUI.Application.DataDictionaries;

public interface IDataDictionaryItemsCache : ITransientDependency
{
    Task<List<DataDictionaryItemDto>> GetOrAddByTypeCodeAsync(
        string typeCode,
        Func<Task<List<DataDictionaryItemDto>>> factory);

    Task RemoveByTypeCodeAsync(string typeCode);

    Task RemoveByTypeIdAsync(Guid typeId);
}