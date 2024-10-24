using Abp.RadzenUI.Utils;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace Abp.RadzenUI.Controllers;

public class ThemeController : AbpControllerBase
{
    [HttpGet("/theme/switch")]
    public IActionResult SwitchAsync(string theme, string returnUrl = "~/")
    {
        AbpRadzenUICookieHelper.SetThemeCookie(HttpContext, theme);
        return Redirect(returnUrl);
    }
}
