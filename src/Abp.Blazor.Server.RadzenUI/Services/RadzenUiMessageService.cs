using Radzen;
using Volo.Abp.AspNetCore.Components.Messages;
using Volo.Abp.DependencyInjection;

namespace Abp.RadzenUI.Services;

public class RadzenUiMessageService(DialogService dialogService)
    : IUiMessageService,
        ITransientDependency
{
    protected DialogService DialogService { get; } = dialogService;

    public async Task Info(string message, string? title = null, Action<UiMessageOptions>? options = null)
    {
        var opts = new UiMessageOptions();
        options?.Invoke(opts);
        
        await DialogService.Alert(message, title ?? "Information", new AlertOptions
        {
            OkButtonText = opts.ConfirmButtonText ?? "OK"
        });
    }

    public async Task Success(string message, string? title = null, Action<UiMessageOptions>? options = null)
    {
        var opts = new UiMessageOptions();
        options?.Invoke(opts);
        
        await DialogService.Alert(message, title ?? "Success", new AlertOptions
        {
            OkButtonText = opts.ConfirmButtonText ?? "OK"
        });
    }

    public async Task Warn(string message, string? title = null, Action<UiMessageOptions>? options = null)
    {
        var opts = new UiMessageOptions();
        options?.Invoke(opts);
        
        await DialogService.Alert(message, title ?? "Warning", new AlertOptions
        {
            OkButtonText = opts.ConfirmButtonText ?? "OK"
        });
    }

    public async Task Error(string message, string? title = null, Action<UiMessageOptions>? options = null)
    {
        var opts = new UiMessageOptions();
        options?.Invoke(opts);

        // We use the title to convey error.
        await DialogService.Alert(message, title ?? "Error", new AlertOptions
        {
            OkButtonText = opts.ConfirmButtonText ?? "OK"
        });
    }

    public async Task<bool> Confirm(string message, string? title = null, Action<UiMessageOptions>? options = null)
    {
        var opts = new UiMessageOptions();
        options?.Invoke(opts);

        var result = await DialogService.Confirm(
            message,
            title ?? "Confirmation",
            new ConfirmOptions
            {
                OkButtonText = opts.ConfirmButtonText ?? "OK",
                CancelButtonText = opts.CancelButtonText ?? "Cancel"
            });
        
        return result ?? false;
    }
}
