using System.Reflection;

namespace Abp.RadzenUI;

public class AbpRadzenUIOptions
{
    public List<Assembly> RouterAdditionalAssemblies { get; set; } = [];

    public TitleBarOptions TitleBar { get; set; } = new();
}

public class TitleBarOptions
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
