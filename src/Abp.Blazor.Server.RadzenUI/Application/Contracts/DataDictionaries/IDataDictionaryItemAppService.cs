using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Abp.RadzenUI.Application.Contracts.DataDictionaries;

public interface IDataDictionaryItemAppService
    : ICrudAppService<DataDictionaryItemDto, Guid, GetDataDictionaryItemsInput, CreateDataDictionaryItemDto, UpdateDataDictionaryItemDto>
{
    /// <summary>
    /// Get all active dictionary items by type code. For business system consumption.
    /// </summary>
    Task<List<DataDictionaryItemDto>> GetItemsByTypeCodeAsync(string typeCode);

    /// <summary>
    /// Get a specific dictionary item by type code and item code. For business system consumption.
    /// </summary>
    Task<DataDictionaryItemDto?> GetItemByTypeCodeAndItemCodeAsync(string typeCode, string itemCode);

    /// <summary>
    /// Toggle the active status of a dictionary item.
    /// </summary>
    Task ToggleActiveAsync(Guid id);
}
