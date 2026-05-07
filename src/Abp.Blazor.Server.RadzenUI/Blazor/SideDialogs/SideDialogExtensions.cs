using Microsoft.AspNetCore.Components;
using Radzen;

namespace Abp.RadzenUI;

public static class SideDialogExtensions
{
    public static Task OpenDetailAsync<TModel, TDetail>(
        this SideDialogCoordinator<TModel> coordinator,
        TModel model,
        string title,
        string parameterName,
        string width
    )
        where TModel : class
        where TDetail : ComponentBase
    {
        return coordinator.OpenIfClosedAsync<TDetail>(
            model,
            title,
            new Dictionary<string, object?> { { parameterName, model } },
            new SideDialogOptions
            {
                ShowMask = false,
                CloseDialogOnOverlayClick = true,
                Width = width
            }
        );
    }
}
