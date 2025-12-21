using Microsoft.AspNetCore.Http;

namespace Abp.RadzenUI.Utils;

public static class AbpRadzenUICookieHelper
{
    private static readonly string ThemeKey = ".AbpRadzenUI.Theme";

    public static void SetThemeCookie(HttpContext context, string theme)
    {
        context.Response.Cookies.Append(ThemeKey, theme, new CookieOptions
        {
            Path = "/",
            HttpOnly = false,
            IsEssential = true,
            Expires = DateTimeOffset.Now.AddYears(10)
        });
    }

    public static string? GetThemeCookie(HttpContext context)
    {
        context.Request.Cookies.TryGetValue(ThemeKey, out var theme);
        return theme;
    }
}
