using Volo.Abp.AspNetCore.Mvc.UI.Bundling;

namespace Abp.RadzenUI.Bundling;

public class BlazorGlobalStyleContributor : BundleContributor
{
    public override void ConfigureBundle(BundleConfigurationContext context)
    {
        context.Files.AddIfNotContains("/_content/Abp.Blazor.Server.RadzenUI/bootstrap/bootstrap.min.css");
        context.Files.AddIfNotContains("/_content/Abp.Blazor.Server.RadzenUI/css/site.css");
        context.Files.AddIfNotContains("/_content/Abp.Blazor.Server.RadzenUI/app.css");

        context.Files.AddIfNotContains("/_content/Abp.Blazor.Server.RadzenUI/fonts/MaterialSymbolsOutlined.woff2");
    }
}
