using System.ComponentModel.DataAnnotations;
using Volo.Abp.Identity;
using Volo.Abp.Validation;

namespace Abp.RadzenUI.Application.Contracts.Users;

public class IdentityUserSetPasswordInput
{
    [Required]
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = string.Empty;
}
