namespace Microsoft.AspNetCore.Builder;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseRadzen404Page(this IApplicationBuilder app)
    {
        app.UseStatusCodePagesWithRedirects("/404");
        return app;
    }
}
