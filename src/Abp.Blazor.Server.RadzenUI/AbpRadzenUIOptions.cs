using System.Reflection;

namespace Abp.RadzenUI;

public class AbpRadzenUIOptions
{
    public List<Assembly> RouterAdditionalAssemblies { get; set; } = [];

    public TitleBarSettings TitleBar { get; set; } = new();

    public LoginPageSettings LoginPage { get; set; } = new();
}

public class LoginPageSettings
{
    public string LogoPath { get; set; } = "_content/Abp.Blazor.Server.RadzenUI/images/radzen.webp";
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
