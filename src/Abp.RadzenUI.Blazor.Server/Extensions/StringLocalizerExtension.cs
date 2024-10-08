using Microsoft.Extensions.Localization;

namespace Volo.Abp.AspNetCore.Components;

public static class StringLocalizerExtension
{
    public static LocalizedString Required(this IStringLocalizer localizer, string code)
    {
        return localizer["The {0} field is required.", localizer[code]];
    }
}
