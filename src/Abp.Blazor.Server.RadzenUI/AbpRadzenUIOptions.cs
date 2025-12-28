using System.Reflection;

namespace Abp.RadzenUI;

public class AbpRadzenUIOptions
{
    public List<Assembly> RouterAdditionalAssemblies { get; set; } = [];

    public TitleBarSettings TitleBar { get; set; } = new();

    public LoginPageSettings LoginPage { get; set; } = new();

    public ThemeSettings Theme { get; set; } = new();

    public ExternalLoginSettings ExternalLogin { get; set; } = new();
}

public class LoginPageSettings
{
    public string Title { get; set; } = "Abp RadzenUI";

    public string LogoPath { get; set; } = "_content/AbpRadzen.Blazor.Server.UI/images/radzen.webp";
}

public class TitleBarSettings
{
    /// <summary>
    /// title bar header name
    /// </summary>
    public string Title { get; set; } = "Abp RadzenUI";

    /// <summary>
    /// Whether to display the GitHub address of the project in the title bar
    /// </summary>
    public bool ShowGithubLink = true;

    /// <summary>
    /// Whether to display multilingual menus
    /// </summary>
    public bool ShowLanguageMenu = true;
}

public class ThemeSettings
{
    public string Default { get; set; } = "material-dark";

    public bool EnablePremiumTheme { get; set; } = false;
}

public class ExternalLoginSettings
{
    public List<ExternalLoginProvider> Providers { get; set; } = [];
}

public class ExternalLoginProvider(string authenticationScheme, string iconPath)
{
    public string AuthenticationScheme { get; set; } = authenticationScheme;

    public string IconPath { get; set; } = iconPath;
}
