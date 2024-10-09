using Microsoft.Extensions.Localization;
using CRM.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace CRM.Blazor;

[Dependency(ReplaceServices = true)]
public class CRMBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<CRMResource> _localizer;

    public CRMBrandingProvider(IStringLocalizer<CRMResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
