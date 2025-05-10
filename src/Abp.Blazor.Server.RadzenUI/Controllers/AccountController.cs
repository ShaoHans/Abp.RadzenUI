using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Account;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Auditing;
using Volo.Abp.Identity;
using Volo.Abp.Identity.AspNetCore;

namespace Abp.RadzenUI.Controllers;

public class AccountController(
    SignInManager<Volo.Abp.Identity.IdentityUser> signInManager,
    IdentitySecurityLogManager identitySecurityLogManager,
    IAccountAppService accountAppService,
    IdentityUserManager userManager,
    IdentityDynamicClaimsPrincipalContributorCache identityDynamicClaimsPrincipalContributorCache
) : AbpControllerBase
{
    [HttpPost("/account/login")]
    [DisableAuditing]
    public async Task<IActionResult> LoginAsync(string username, string password, bool rememberMe)
    {
        var result = await signInManager.PasswordSignInAsync(username, password, rememberMe, true);

        await identitySecurityLogManager.SaveAsync(
            new IdentitySecurityLogContext()
            {
                Identity = IdentitySecurityLogIdentityConsts.Identity,
                Action = result.ToIdentitySecurityLogAction(),
                UserName = username
            }
        );

        if (result.Succeeded)
        {
            return Redirect("~/");
        }

        return Redirect($"~/Login?error=Login Failed:{result.GetResultAsString()}");
    }

    [HttpPost("/account/externallogin")]
    public async Task<IActionResult> ExternalLoginAsync(string provider, string returnUrl)
    {
        var properties = signInManager.ConfigureExternalAuthenticationProperties(
            provider,
            returnUrl
        );
        properties.Items["scheme"] = provider;

        return await Task.FromResult(Challenge(properties, provider));
    }

    [HttpGet("/account/logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        await signInManager.SignOutAsync();
        await HttpContext.SignOutAsync();
        return Redirect("~/Login");
    }

    [HttpPost("/account/register")]
    public async Task<IActionResult> RegisterAsync(RegisterDto registerDto)
    {
        registerDto.AppName = "BlazorServer WebApp";
        var userDto = await accountAppService.RegisterAsync(registerDto);

        var user = await userManager.GetByIdAsync(userDto.Id);
        await signInManager.SignInAsync(user, isPersistent: true);

        // Clear the dynamic claims cache.
        await identityDynamicClaimsPrincipalContributorCache.ClearAsync(user.Id, user.TenantId);

        return Redirect("~/");
    }
}
