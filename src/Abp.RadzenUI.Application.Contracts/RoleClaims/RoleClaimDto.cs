namespace Abp.RadzenUI.Application.Contracts.RoleClaims;

public class RoleClaimDto
{
    public string ClaimType { get; set; } = default!;
    public string ClaimValue { get; set; } = default!;
}
