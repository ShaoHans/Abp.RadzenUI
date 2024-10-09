using Volo.Abp.AspNetCore.Mvc.UI.Bundling;

namespace Abp.RadzenUI.Bundling;

public class BlazorGlobalStyleContributor : BundleContributor
{
    public override void ConfigureBundle(BundleConfigurationContext context)
    {
        context.Files.AddIfNotContains("/_content/Abp.RadzenUI.Blazor.Server/bootstrap/bootstrap.min.css");
        context.Files.AddIfNotContains("/_content/Abp.RadzenUI.Blazor.Server/css/fluent-base.css");
        context.Files.AddIfNotContains("/_content/Abp.RadzenUI.Blazor.Server/css/fluent-dark-base.css");
        context.Files.AddIfNotContains("/_content/Abp.RadzenUI.Blazor.Server/css/fluent-dark-wcag.css");
        context.Files.AddIfNotContains("/_content/Abp.RadzenUI.Blazor.Server/css/fluent-wcag.css");
        context.Files.AddIfNotContains("/_content/Abp.RadzenUI.Blazor.Server/css/material3-base.css");
        context.Files.AddIfNotContains("/_content/Abp.RadzenUI.Blazor.Server/css/material3-dark-base.css");
        context.Files.AddIfNotContains("/_content/Abp.RadzenUI.Blazor.Server/css/material3-dark-wcag.css");
        context.Files.AddIfNotContains("/_content/Abp.RadzenUI.Blazor.Server/css/material3-wcag.css");
        context.Files.AddIfNotContains("/_content/Abp.RadzenUI.Blazor.Server/css/site.css");
        context.Files.AddIfNotContains("/_content/Abp.RadzenUI.Blazor.Server/app.css");

        context.Files.AddIfNotContains("/_content/Abp.RadzenUI.Blazor.Server/fonts/MaterialSymbolsOutlined.woff2");

    }
}
