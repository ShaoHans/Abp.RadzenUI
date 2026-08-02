using Abp.RadzenUI.Application.Contracts.Users;
using Abp.RadzenUI.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;

namespace Abp.RadzenUI.Application.Users;

[Authorize(IdentityPermissions.Users.Update)]
public class UserManagementAppService : ApplicationService, IUserManagementAppService
{
    protected IdentityUserManager UserManager { get; }

    public UserManagementAppService(IdentityUserManager userManager)
    {
        UserManager = userManager;
        LocalizationResource = typeof(AbpRadzenUIResource);
    }

    public virtual async Task LockAsync(Guid id, LockUserInput input)
    {
        if (input.LockoutEnd <= DateTimeOffset.UtcNow)
        {
            throw new UserFriendlyException(L["User:LockoutEndMustBeFuture"]);
        }

        var user = await UserManager.GetByIdAsync(id);

        // SetLockoutEndDateAsync only takes effect when lockout is enabled for the user,
        // so ensure it is on before writing the end date.
        (await UserManager.SetLockoutEnabledAsync(user, true)).CheckErrors();
        (await UserManager.SetLockoutEndDateAsync(user, input.LockoutEnd)).CheckErrors();
    }

    public virtual async Task UnlockAsync(Guid id)
    {
        var user = await UserManager.GetByIdAsync(id);
        (await UserManager.SetLockoutEndDateAsync(user, null)).CheckErrors();
    }

    public virtual async Task SetPasswordAsync(Guid id, IdentityUserSetPasswordInput input)
    {
        var user = await UserManager.GetByIdAsync(id);

        if (await UserManager.HasPasswordAsync(user))
        {
            (await UserManager.RemovePasswordAsync(user)).CheckErrors();
        }

        (await UserManager.AddPasswordAsync(user, input.NewPassword)).CheckErrors();

        // The admin has just set a concrete password, so clear the "must change password on next
        // login" flag. Otherwise AbpSignInManager.PreSignInCheck blocks the login with NotAllowed,
        // and the open-source Account module has no "change password" flow to clear it.
        user.SetShouldChangePasswordOnNextLogin(false);

        (await UserManager.UpdateAsync(user)).CheckErrors();
    }
}
