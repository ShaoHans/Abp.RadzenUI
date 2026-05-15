using Volo.Abp.Application.Services;

namespace Abp.RadzenUI.Application.Contracts.Settings;

public interface IAccountSettingsAppService : IApplicationService 
{
    Task<AccountSettingsDto> GetAsync();

    Task UpdateAsync(UpdateAccountSettingsDto input);
}
