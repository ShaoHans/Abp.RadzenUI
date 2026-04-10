using System.Security.Claims;
using Abp.RadzenUI.Application.Contracts.RoleClaims;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;

namespace Abp.RadzenUI.Application;

public class RoleClaimAppService : ApplicationService, IRoleClaimAppService
{
    protected IdentityRoleManager RoleManager { get; }

    public RoleClaimAppService(IdentityRoleManager roleManager)
    {
        RoleManager = roleManager;
    }

    public virtual async Task<List<RoleClaimDto>> GetClaimsAsync(Guid roleId)
    {
        var role = await RoleManager.GetByIdAsync(roleId);
        var claims = await RoleManager.GetClaimsAsync(role);

        return claims
            .Select(c => new RoleClaimDto { ClaimType = c.Type, ClaimValue = c.Value })
            .ToList();
    }

    public virtual async Task AddClaimAsync(Guid roleId, CreateRoleClaimDto input)
    {
        var role = await RoleManager.GetByIdAsync(roleId);
        var claim = new Claim(input.ClaimType, input.ClaimValue);

        var result = await RoleManager.AddClaimAsync(role, claim);
        if (!result.Succeeded)
        {
            throw new AbpException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    public virtual async Task DeleteClaimAsync(Guid roleId, RoleClaimDto input)
    {
        var role = await RoleManager.GetByIdAsync(roleId);
        var claim = new Claim(input.ClaimType, input.ClaimValue);

        var result = await RoleManager.RemoveClaimAsync(role, claim);
        if (!result.Succeeded)
        {
            throw new AbpException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
