using Abp.RadzenUI.Application.LocalizationTexts;
using Microsoft.Extensions.Options;
using Volo.Abp.Localization;

namespace Abp.RadzenUI.Infrastructure.Services;

/// <summary>
/// Attaches the database-backed <see cref="DbLocalizationResourceContributor"/> to every localization
/// resource so that runtime texts can be overridden online. Runs after all <c>Configure</c> delegates,
/// so every resource registered by any module is present. The contributor is added last, giving its
/// (database) values precedence over the static JSON baseline.
/// </summary>
public class AbpRadzenUILocalizationPostConfigureOptions(IOptions<AbpRadzenUIOptions> radzenUIOptions)
    : IPostConfigureOptions<AbpLocalizationOptions>
{
    public void PostConfigure(string? name, AbpLocalizationOptions options)
    {
        if (!radzenUIOptions.Value.EnableLocalizationManagement)
        {
            return;
        }

        foreach (var resource in options.Resources.Values)
        {
            resource.Contributors.Add(new DbLocalizationResourceContributor());
        }
    }
}
