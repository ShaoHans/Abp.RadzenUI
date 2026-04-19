using Abp.RadzenUI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Abp.RadzenUI.Controllers;

public class LinkedAccountsController(LinkedAccountSignInManager linkedAccountSignInManager)
    : AbpRadzenControllerBase
{
    [HttpGet("/account/linked-accounts/link-login")]
    public async Task<IActionResult> LinkLoginAsync(string token, CancellationToken cancellationToken)
    {
        var redirectUrl = await linkedAccountSignInManager.BeginLinkLoginAsync(token, cancellationToken);
        return Redirect(redirectUrl);
    }

    [HttpGet("/account/linked-accounts/switch")]
    public async Task<IActionResult> SwitchAsync(string token, CancellationToken cancellationToken)
    {
        var redirectUrl = await linkedAccountSignInManager.SwitchAsync(token, cancellationToken);
        return Redirect(redirectUrl);
    }

    [HttpGet("/account/linked-accounts/back")]
    public async Task<IActionResult> BackAsync(string? returnUrl = null, CancellationToken cancellationToken = default)
    {
        var redirectUrl = await linkedAccountSignInManager.BackAsync(returnUrl, cancellationToken);
        return Redirect(redirectUrl);
    }
}