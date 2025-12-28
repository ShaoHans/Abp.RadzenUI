using Abp.RadzenUI;
using Abp.RadzenUI.UIPlaceHolders;
using CRM.MultiTenancy;
using Microsoft.Extensions.Options;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;

namespace CRM.Blazor.Web;

public class CRMUIPlaceHolderResolver(
    ILogger<CRMUIPlaceHolderResolver> logger,
    ICurrentTenant currentTenant,
    ITenantRepository tenantRepository,
    IOptionsMonitor<AbpRadzenUIOptions> radzenUIOptions
) : IUIPlaceHolderResolver
{
    private readonly AbpRadzenUIOptions _radzenUIOptions = radzenUIOptions.CurrentValue;

    public async Task<LoginPageSettings> GetLoginPageSettingsAsync()
    {
        var displayName = await GetTenantDisplayNameAsync();
        if (!string.IsNullOrEmpty(displayName))
        {
            return new LoginPageSettings
            {
                Title = displayName,
                LogoPath = _radzenUIOptions.LoginPage.LogoPath
            };
        }
        return _radzenUIOptions.LoginPage;
    }

    public async Task<TitleBarSettings> GetTitleBarSettingsAsync()
    {
        var displayName = await GetTenantDisplayNameAsync();
        if (!string.IsNullOrEmpty(displayName))
        {
            return new TitleBarSettings
            {
                Title = displayName,
                ShowGithubLink = _radzenUIOptions.TitleBar.ShowGithubLink,
                ShowLanguageMenu = _radzenUIOptions.TitleBar.ShowLanguageMenu
            };
        }
        return _radzenUIOptions.TitleBar;
    }

    private async Task<string?> GetTenantDisplayNameAsync()
    {
        try
        {
            if (currentTenant.IsAvailable)
            {
                var tenant = await tenantRepository.GetAsync(currentTenant.Id!.Value);
                return tenant.GetProperty<string>(MultiTenancyConsts.TenantDisplayNameField);
            }

            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get tenant display name.");
            return null;
        }
    }
}
