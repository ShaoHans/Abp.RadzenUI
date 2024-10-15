using Volo.Abp.AspNetCore.Mvc.UI.Bundling;

namespace Abp.RadzenUI.Bundling;

public class BlazorGlobalStyleContributor : BundleContributor
{
    public override void ConfigureBundle(BundleConfigurationContext context)
    {
        context.Files.AddIfNotContains("/_content/AbpRadzen.Blazor.Server.UI/bootstrap/bootstrap.min.css");
        context.Files.AddIfNotContains("/_content/AbpRadzen.Blazor.Server.UI/css/site.css");
        context.Files.AddIfNotContains("/_content/AbpRadzen.Blazor.Server.UI/app.css");

        context.Files.AddIfNotContains("/_content/AbpRadzen.Blazor.Server.UI/fonts/MaterialSymbolsOutlined.woff2");
    }
}
