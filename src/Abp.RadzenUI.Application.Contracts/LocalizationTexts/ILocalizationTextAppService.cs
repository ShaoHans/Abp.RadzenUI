using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Abp.RadzenUI.Application.Contracts.LocalizationTexts;

public interface ILocalizationTextAppService : IApplicationService
{
    /// <summary>
    /// Lists the localization resources that can be managed.
    /// </summary>
    Task<ListResultDto<LocalizationResourceInfoDto>> GetResourcesAsync();

    /// <summary>
    /// Lists the configured cultures.
    /// </summary>
    Task<ListResultDto<LocalizationCultureDto>> GetCulturesAsync();

    /// <summary>
    /// Lists the texts of a resource/culture, merging the static baseline with database overrides.
    /// </summary>
    Task<PagedResultDto<LocalizationTextItemDto>> GetListAsync(GetLocalizationTextsInput input);

    /// <summary>
    /// Creates or updates a database override for a single key.
    /// </summary>
    Task SaveAsync(SaveLocalizationTextDto input);

    /// <summary>
    /// Removes the database override for a single key, reverting to the static baseline.
    /// </summary>
    Task ResetAsync(ResetLocalizationTextDto input);
}
