using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Validation;

namespace Abp.RadzenUI.Controllers;

public class AbpRadzenControllerBase : AbpControllerBase
{
    protected virtual IActionResult RedirectWithError(string url, Exception ex)
    {
        return Redirect($"{url}?error=" + GetErrorMessage(ex));
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
}
