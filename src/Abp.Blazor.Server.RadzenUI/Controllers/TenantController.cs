using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.MultiTenancy;
using Volo.Abp.MultiTenancy.Localization;

namespace Abp.RadzenUI.Controllers;

public class TenantController : AbpControllerBase
{
    protected ITenantStore TenantStore { get; }
    protected ITenantNormalizer TenantNormalizer { get; }
    protected AbpAspNetCoreMultiTenancyOptions Options { get; }

    public TenantController(
        ITenantStore tenantStore,
        ITenantNormalizer tenantNormalizer,
        IOptions<AbpAspNetCoreMultiTenancyOptions> options
    )
    {
        TenantStore = tenantStore;
        TenantNormalizer = tenantNormalizer;
        Options = options.Value;
        LocalizationResource = typeof(AbpMultiTenancyResource);
    }

    [HttpGet("/tenant/switch")]
    public IActionResult SwitchAsync(Guid? tenantId = null, string? returnUrl = null)
    {
        AbpMultiTenancyCookieHelper.SetTenantCookie(HttpContext, tenantId, Options.TenantKey);

        if (!returnUrl.IsNullOrWhiteSpace())
        {
            if (Uri.TryCreate(returnUrl, UriKind.Absolute, out var absoluteUri))
            {
                returnUrl = absoluteUri.PathAndQuery + absoluteUri.Fragment;
            }

            if (returnUrl.StartsWith("~"))
            {
                return Redirect(returnUrl);
            }

            returnUrl = returnUrl.EnsureStartsWith('/');
            return Redirect(returnUrl);
        }

        return Redirect("~/account/login");
    }
}
