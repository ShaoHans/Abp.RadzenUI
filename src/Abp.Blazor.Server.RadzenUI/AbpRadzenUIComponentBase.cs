using Volo.Abp.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.ExceptionHandling;

namespace Abp.RadzenUI;

public abstract class AbpRadzenUIComponentBase : AbpComponentBase
{
    protected override async Task HandleErrorAsync(Exception exception)
    {
        if (!base.IsDisposed)
        {
            await InvokeAsync(async () =>
            {
                await UserExceptionInformer.InformAsync(
                    new UserExceptionInformerContext(exception)
                );
            });

            // Call StateHasChanged separately within the InvokeAsync context
            await InvokeAsync(StateHasChanged);
        }
    }
}
