using System.ComponentModel.DataAnnotations;

namespace Abp.RadzenUI.Application.Contracts.RoleClaims;

public class CreateRoleClaimDto
{
    [Required]
    [MaxLength(256)]
    public string ClaimType { get; set; } = default!;

    [Required]
    [MaxLength(1024)]
    public string ClaimValue { get; set; } = default!;
}
