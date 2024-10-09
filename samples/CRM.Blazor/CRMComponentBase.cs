using CRM.Localization;
using Volo.Abp.AspNetCore.Components;

namespace CRM.Blazor;

public abstract class CRMComponentBase : AbpComponentBase
{
    protected CRMComponentBase()
    {
        LocalizationResource = typeof(CRMResource);
    }
}
