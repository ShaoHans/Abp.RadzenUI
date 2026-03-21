using Microsoft.AspNetCore.Http;

namespace Abp.RadzenUI.Utils;

public static class AbpRadzenUICookieHelper
{
    public const string ThemeKey = ".AbpRadzenUI.Theme";
    public const string PageSizeKey = ".AbpRadzenUI.PageSize";

    public static void SetThemeCookie(HttpContext context, string theme)
    {
        SetCookie(context, ThemeKey, theme);
    }

    public static string? GetThemeCookie(HttpContext? context)
    {
        if (context is null)
        {
            return null;
        }

        context.Request.Cookies.TryGetValue(ThemeKey, out var theme);
        return theme;
    }

    public static void SetPageSizeCookie(HttpContext context, int pageSize)
    {
        SetCookie(context, PageSizeKey, pageSize.ToString());
    }

    public static int? GetPageSizeCookie(HttpContext? context)
    {
        if (context is null)
        {
            return null;
        }

        return context.Request.Cookies.TryGetValue(PageSizeKey, out var value)
            && int.TryParse(value, out var pageSize)
            ? pageSize
            : null;
    }

    private static void SetCookie(HttpContext context, string key, string value)
    {
        context.Response.Cookies.Append(key, value, new CookieOptions
        {
            Path = "/",
            HttpOnly = false,
            IsEssential = true,
            Expires = DateTimeOffset.Now.AddYears(10)
        });
    }
}
