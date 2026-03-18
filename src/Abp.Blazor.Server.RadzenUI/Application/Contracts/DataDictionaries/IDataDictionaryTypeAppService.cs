using System;
using Volo.Abp.Application.Services;

namespace Abp.RadzenUI.Application.Contracts.DataDictionaries;

public interface IDataDictionaryTypeAppService
    : ICrudAppService<DataDictionaryTypeDto, Guid, GetDataDictionaryTypesInput, CreateDataDictionaryTypeDto, UpdateDataDictionaryTypeDto>
{
}
