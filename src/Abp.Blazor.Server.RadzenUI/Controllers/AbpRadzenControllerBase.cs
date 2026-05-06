using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.Validation;

namespace Abp.RadzenUI.Controllers;

public class AbpRadzenControllerBase : AbpControllerBase
{
    protected IAppUrlProvider AppUrlProvider =>
        LazyServiceProvider.LazyGetRequiredService<IAppUrlProvider>();

    protected virtual IActionResult RedirectWithError(string url, string error)
    {
        return Redirect(QueryHelpers.AddQueryString(url, "error", SanitizeForHeader(error)));
    }

    protected virtual IActionResult RedirectWithError(string url, Exception ex)
    {
        return RedirectWithError(url, GetErrorMessage(ex));
    }

    protected virtual string GetErrorMessage(Exception ex)
    {
        Logger.LogError(ex, "internal error.");

        string error = ex.Message;
        if (ex is AbpValidationException validationEx)
        {
            error = string.Join(" ", validationEx.ValidationErrors.Select(x => x.ErrorMessage));
        }
        else if (ex is UserFriendlyException friendlyException)
        {
            error = friendlyException.Message;
        }

        return error;
    }

    protected virtual string SanitizeForHeader(string? value)
    {
        if (value.IsNullOrEmpty())
        {
            return string.Empty;
        }

        return new string(value.Where(ch => !char.IsControl(ch)).ToArray());
    }

    protected virtual async Task<RedirectResult> RedirectSafelyAsync(
        string returnUrl,
        string? returnUrlHash = null
    )
    {
        return Redirect(await GetRedirectUrlAsync(returnUrl, returnUrlHash));
    }

    protected virtual async Task<string> GetRedirectUrlAsync(
        string returnUrl,
        string? returnUrlHash = null
    )
    {
        returnUrl = await NormalizeReturnUrlAsync(returnUrl);

        if (!returnUrlHash.IsNullOrWhiteSpace())
        {
            returnUrl += returnUrlHash;
        }

        return returnUrl;
    }

    protected virtual async Task<string> NormalizeReturnUrlAsync(string returnUrl)
    {
        if (returnUrl.IsNullOrEmpty())
        {
            return await GetAppHomeUrlAsync();
        }

        if (Url.IsLocalUrl(returnUrl) || await AppUrlProvider.IsRedirectAllowedUrlAsync(returnUrl))
        {
            return returnUrl;
        }

        return await GetAppHomeUrlAsync();
    }

    protected virtual Task<string> GetAppHomeUrlAsync()
    {
        return Task.FromResult("~/"); //TODO: ???
    }
}
