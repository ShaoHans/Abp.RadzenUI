using Microsoft.AspNetCore.Components;
using Radzen;

namespace Abp.RadzenUI;

public sealed class SideDialogCoordinator<T>
    : IDisposable
    where T : class
{
    private readonly DialogService _dialogService;
    private readonly SideDialogState<T> _dialogState;
    private bool _sideOpen;
    private Type? _openedComponentType;

    public SideDialogCoordinator(DialogService dialogService, SideDialogState<T> dialogState)
    {
        _dialogService = dialogService;
        _dialogState = dialogState;
        _dialogService.OnSideClose += OnSideClosed;
        _dialogService.OnSideOpen += OnSideOpened;
    }

    public async Task OpenIfClosedAsync<TComponent>(
        T value,
        string title,
        Dictionary<string, object?> parameters,
        SideDialogOptions options
    )
        where TComponent : ComponentBase
    {
        await _dialogState.SetSelectedAsync(value);

        if (!_sideOpen)
        {
            await _dialogService.OpenSideAsync<TComponent>(title, parameters, options);
            return;
        }

        if (_openedComponentType == typeof(TComponent))
        {
            return;
        }

        await _dialogService.OpenSideAsync<TComponent>(title, parameters, options);
    }

    private void OnSideOpened(
        Type componentType,
        Dictionary<string, object?> parameters,
        SideDialogOptions options
    )
    {
        _sideOpen = true;
        _openedComponentType = componentType;
    }

    private void OnSideClosed(dynamic _)
    {
        _sideOpen = false;
        _openedComponentType = null;
    }

    public void Dispose()
    {
        _dialogService.OnSideClose -= OnSideClosed;
        _dialogService.OnSideOpen -= OnSideOpened;
    }
}
