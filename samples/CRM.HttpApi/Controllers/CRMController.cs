using CRM.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace CRM.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class CRMController : AbpControllerBase
{
    protected CRMController()
    {
        LocalizationResource = typeof(CRMResource);
    }
}
