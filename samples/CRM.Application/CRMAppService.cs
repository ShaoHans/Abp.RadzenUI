using CRM.Localization;
using Volo.Abp.Application.Services;

namespace CRM;

/* Inherit your application services from this class.
 */
public abstract class CRMAppService : ApplicationService
{
    protected CRMAppService()
    {
        LocalizationResource = typeof(CRMResource);
    }
}
