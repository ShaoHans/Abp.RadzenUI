using Volo.Abp.Application.Services;

namespace Abp.RadzenUI.Application.Contracts.Dashboard;

public interface IDashboardAppService : IApplicationService
{
    Task<DashboardDto> GetAsync();
}
