using Volo.Abp.Application.Services;

namespace Abp.RadzenUI.Application.Contracts.Tenants;

public interface ITenantConnectionStringAppService : IApplicationService
{
    Task<List<TenantConnectionStringDto>> GetListAsync(Guid tenantId);

    Task AddOrUpdateAsync(Guid tenantId, TenantConnectionStringDto input);

    Task DeleteAsync(Guid tenantId, string name);
}
