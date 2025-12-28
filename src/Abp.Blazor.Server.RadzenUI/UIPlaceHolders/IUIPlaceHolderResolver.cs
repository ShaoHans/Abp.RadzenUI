using Volo.Abp.DependencyInjection;

namespace Abp.RadzenUI.UIPlaceHolders;

public interface IUIPlaceHolderResolver : ITransientDependency
{
    Task<LoginPageSettings> GetLoginPageSettingsAsync();

    Task<TitleBarSettings> GetTitleBarSettingsAsync();
}
