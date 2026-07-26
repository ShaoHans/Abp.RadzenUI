using Volo.Abp.Application.Services;

namespace Abp.RadzenUI.Application.Contracts.Users;

/// <summary>
/// User management actions that the stock <c>IIdentityUserAppService</c> does not expose:
/// one-click lock / unlock (operating on <c>LockoutEnd</c>) and admin-driven password reset.
/// </summary>
public interface IUserManagementAppService : IApplicationService
{
    /// <summary>Locks the user out until <see cref="LockUserInput.LockoutEnd"/> (also ensures lockout is enabled).</summary>
    Task LockAsync(Guid id, LockUserInput input);

    /// <summary>Clears the lockout end date so the user can sign in again.</summary>
    Task UnlockAsync(Guid id);

    /// <summary>Sets a new password for the user without requiring the current one (admin reset).</summary>
    Task SetPasswordAsync(Guid id, IdentityUserSetPasswordInput input);
}
