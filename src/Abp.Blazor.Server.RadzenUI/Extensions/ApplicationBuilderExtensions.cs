using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Builder;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseRadzenUI(this IApplicationBuilder app)
    {
        app.UseStatusCodePagesWithRedirects("/404");
        app.UseConfiguredEndpoints(builder =>
        {
            builder.MapRadzenUI();
        });
        return app;
    }
}
