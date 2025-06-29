using Abp.RadzenUI;
using Abp.RadzenUI.Components;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Routing;

public static class EndpointRouteBuilderExtensions
{
    public static RazorComponentsEndpointConventionBuilder MapRadzenUI(
        this IEndpointRouteBuilder builder
    )
    {
        return builder
            .MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(
                [
                    .. builder
                        .ServiceProvider.GetRequiredService<IOptions<AbpRadzenUIOptions>>()
                        .Value.RouterAdditionalAssemblies,
                ]
            );
    }
}
