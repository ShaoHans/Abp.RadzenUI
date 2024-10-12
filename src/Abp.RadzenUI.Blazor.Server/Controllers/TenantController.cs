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
    public IActionResult SwitchAsync(Guid? tenantId = null)
    {
        AbpMultiTenancyCookieHelper.SetTenantCookie(HttpContext, tenantId, Options.TenantKey);
        return Redirect("~/Login");
    }
}
