using System.Reflection;
using Abp.RadzenUI.Components;

namespace Microsoft.AspNetCore.Builder;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseRadzenUI(
        this IApplicationBuilder app,
        params Assembly[] assemblies
    )
    {
        app.UseStatusCodePagesWithRedirects("/404");
        ((WebApplication)app)
            .MapRazorComponents<App>()
            .AddAdditionalAssemblies(assemblies)
            .AddInteractiveServerRenderMode();

        return app;
    }
}
