using Microsoft.Extensions.Localization;

namespace Abp.RadzenUI.Utils;

public static class UiLocalizationHelper
{
    public static string GetDisplayName(
        string name,
        string? localizationKey,
        IStringLocalizerFactory factory,
        string prefix = "DisplayName:")
    {
        var localizer = factory.CreateDefaultOrNull();
        if (localizer == null)
            return name;

        if (!localizationKey.IsNullOrEmpty())
        {
            var value = localizer[localizationKey];
            if (!value.ResourceNotFound)
                return value;
        }

        var fallback = localizer[$"{prefix}{name}"];
        if (!fallback.ResourceNotFound)
            return fallback;

        return name;
    }
}

