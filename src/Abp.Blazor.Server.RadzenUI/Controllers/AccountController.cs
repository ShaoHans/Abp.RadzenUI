﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Auditing;
using Volo.Abp.Identity;
using Volo.Abp.Identity.AspNetCore;

namespace Abp.RadzenUI.Controllers;

public class AccountController : AbpControllerBase
{
    private readonly SignInManager<Volo.Abp.Identity.IdentityUser> _signInManager;
    private readonly IdentitySecurityLogManager _identitySecurityLogManager;

    public AccountController(
        SignInManager<Volo.Abp.Identity.IdentityUser> signInManager,
        IdentitySecurityLogManager identitySecurityLogManager
    )
    {
        _signInManager = signInManager;
        _identitySecurityLogManager = identitySecurityLogManager;
    }

    [HttpPost("/account/login")]
    [DisableAuditing]
    public async Task<IActionResult> LoginAsync(string username, string password, bool rememberMe)
    {
        var result = await _signInManager.PasswordSignInAsync(username, password, rememberMe, true);

        await _identitySecurityLogManager.SaveAsync(
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

    [HttpGet("/account/logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        await _signInManager.SignOutAsync();
        await HttpContext.SignOutAsync();
        return Redirect("~/Login");
    }
}
