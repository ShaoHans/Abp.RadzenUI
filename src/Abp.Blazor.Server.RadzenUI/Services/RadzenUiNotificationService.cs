using Radzen;
using Volo.Abp.AspNetCore.Components.Messages;
using Volo.Abp.AspNetCore.Components.Notifications;
using Volo.Abp.DependencyInjection;

namespace Abp.RadzenUI.Services;

public class RadzenUiNotificationService(NotificationService notificationService)
    : IUiNotificationService,
        ITransientDependency
{
    protected NotificationService NotificationService { get; } = notificationService;
    
    public Task Info(string message, string? title = null, Action<UiNotificationOptions>? options = null)
    {
        NotificationService.Info(message, title);
        return Task.CompletedTask;
    }

    public Task Success(string message, string? title = null, Action<UiNotificationOptions>? options = null)
    {
        NotificationService.Success(message, title);
        return Task.CompletedTask;
    }

    public Task Warn(string message, string? title = null, Action<UiNotificationOptions>? options = null)
    {
        NotificationService.Warning(message, title);
        return Task.CompletedTask;
    }

    public Task Error(string message, string? title = null, Action<UiNotificationOptions>? options = null)
    {
        NotificationService.Error(message, title);
        return Task.CompletedTask;
    }
}
