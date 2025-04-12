﻿using Radzen;
using Volo.Abp.AspNetCore.Components.Messages;
using Volo.Abp.DependencyInjection;

namespace Abp.RadzenUI.Services;

public class RadzenUiMessageService(NotificationService notificationService)
    : IUiMessageService,
        ITransientDependency
{
    protected NotificationService NotificationService { get; } = notificationService;

    public Task Info(string message, string? title = null, Action<UiMessageOptions>? options = null)
    {
        NotificationService.Info(message, title);
        return Task.CompletedTask;
    }

    public Task Success(
        string message,
        string? title = null,
        Action<UiMessageOptions>? options = null
    )
    {
        NotificationService.Success(message, title);
        return Task.CompletedTask;
    }

    public Task Warn(string message, string? title = null, Action<UiMessageOptions>? options = null)
    {
        NotificationService.Warning(message, title);
        return Task.CompletedTask;
    }

    public Task Error(
        string message,
        string? title = null,
        Action<UiMessageOptions>? options = null
    )
    {
        NotificationService.Error(message, title);
        return Task.CompletedTask;
    }

    public Task<bool> Confirm(
        string message,
        string? title = null,
        Action<UiMessageOptions>? options = null
    )
    {
        return Task.FromResult(true);
    }
}
