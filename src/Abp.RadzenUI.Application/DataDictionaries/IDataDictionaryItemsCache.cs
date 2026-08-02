using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.RadzenUI.Application.Contracts.DataDictionaries;
using Abp.RadzenUI.DataDictionaries;
using Volo.Abp.DependencyInjection;

namespace Abp.RadzenUI.Application.DataDictionaries;

public interface IDataDictionaryItemsCache : ITransientDependency
{
    Task<List<DataDictionaryItemDto>> GetOrAddByTypeAsync(
        DataDictionaryType type,
        Func<Task<List<DataDictionaryItemDto>>> factory);

    Task RemoveByTypeIdAsync(Guid typeId);
}