using Volo.Abp.Application.Services;

namespace Abp.RadzenUI.Application.Contracts.RoleClaims;

public interface IRoleClaimAppService : IApplicationService
{
    Task<List<RoleClaimDto>> GetClaimsAsync(Guid roleId);
    Task AddClaimAsync(Guid roleId, CreateRoleClaimDto input);
    Task DeleteClaimAsync(Guid roleId, RoleClaimDto input);
}
