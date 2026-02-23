using Microsoft.Extensions.Options;

namespace Abp.RadzenUI.UIPlaceHolders;

public class DefaultUIPlaceHolderResolver(IOptionsMonitor<AbpRadzenUIOptions> radzenUIOptions)
    : IUIPlaceHolderResolver
{
    private readonly AbpRadzenUIOptions _radzenUIOptions = radzenUIOptions.CurrentValue;

    public Task<LoginPageSettings> GetLoginPageSettingsAsync()
    {
        return Task.FromResult(_radzenUIOptions.LoginPage);
    }

    public Task<TitleBarSettings> GetTitleBarSettingsAsync()
    {
        return Task.FromResult(_radzenUIOptions.TitleBar);
    }
}
