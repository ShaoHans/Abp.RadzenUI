using System.Reflection;

namespace Abp.RadzenUI;

public class AbpRadzenUIOptions
{
    public List<Assembly> RouterAdditionalAssemblies { get; set; } = [];

    /// <summary>
    /// Whether to display the GitHub address of the project in the title bar
    /// </summary>
    public bool ShowGithubLink = true;
}
