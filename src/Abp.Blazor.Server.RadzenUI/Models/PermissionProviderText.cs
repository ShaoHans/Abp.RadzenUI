using Microsoft.Extensions.Localization;
using Volo.Abp.PermissionManagement;

namespace Abp.RadzenUI.Models;

/// <summary>
/// Readable label for a permission grant source, e.g. "Role: admin" instead of "R: admin".
/// </summary>
public static class PermissionProviderText
{
    /// <param name="localizer">Localizer of the AbpRadzenUI resource.</param>
    /// <param name="provider">The grant source.</param>
    public static string Format(IStringLocalizer localizer, ProviderInfoDto provider)
    {
        // ABP's built-in provider names are single letters (R = role, U = user, C = client);
        // show a readable word for those and keep any custom provider name as-is.
        var name = localizer[$"PermissionVisualizer:Provider.{provider.ProviderName}"];
        var providerText = name.ResourceNotFound ? provider.ProviderName : name.Value;
        return $"{providerText}: {provider.ProviderKey}";
    }
}
