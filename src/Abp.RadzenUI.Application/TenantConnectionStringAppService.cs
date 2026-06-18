using Abp.RadzenUI.Application.Contracts.Tenants;
using System.Data.Common;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp;
using Volo.Abp.TenantManagement;

namespace Abp.RadzenUI.Application;

public class TenantConnectionStringAppService
    : ApplicationService, ITenantConnectionStringAppService
{
    private readonly ITenantRepository _tenantRepository;

    public TenantConnectionStringAppService(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<List<TenantConnectionStringDto>> GetListAsync(Guid tenantId)
    {
        await AuthorizationService.CheckAsync(
            TenantManagementPermissions.Tenants.ManageConnectionStrings
        );

        var tenant = await _tenantRepository.GetAsync(tenantId, includeDetails: true);
        return tenant
            .ConnectionStrings.Select(cs => new TenantConnectionStringDto
            {
                Name = cs.Name,
                Value = cs.Value,
            })
            .ToList();
    }

    public async Task AddOrUpdateAsync(Guid tenantId, TenantConnectionStringDto input)
    {
        await AuthorizationService.CheckAsync(
            TenantManagementPermissions.Tenants.ManageConnectionStrings
        );

        ValidateConnectionString(input.Value);

        var tenant = await _tenantRepository.GetAsync(tenantId, includeDetails: true);
        tenant.SetConnectionString(input.Name, input.Value);
        await _tenantRepository.UpdateAsync(tenant);
    }

    public async Task DeleteAsync(Guid tenantId, string name)
    {
        await AuthorizationService.CheckAsync(
            TenantManagementPermissions.Tenants.ManageConnectionStrings
        );

        var tenant = await _tenantRepository.GetAsync(tenantId, includeDetails: true);
        tenant.RemoveConnectionString(name);
        await _tenantRepository.UpdateAsync(tenant);
    }

    private static void ValidateConnectionString(string connectionString)
    {
        try
        {
            _ = new DbConnectionStringBuilder { ConnectionString = connectionString };
        }
        catch (ArgumentException ex)
        {
            throw new UserFriendlyException(
                $"Invalid connection string format: {ex.Message}"
            );
        }
    }
}
