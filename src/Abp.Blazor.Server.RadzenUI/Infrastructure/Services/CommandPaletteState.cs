namespace Abp.RadzenUI;

/// <summary>
/// Coordinates opening the command palette from anywhere (header button, Ctrl+K JS
/// handler) with the single <c>CommandPalette</c> component that owns the overlay.
/// </summary>
public sealed class CommandPaletteState
{
    public event Func<Task>? OpenRequested;

    public async Task OpenAsync()
    {
        if (OpenRequested is null)
        {
            return;
        }

        foreach (var handler in OpenRequested.GetInvocationList().Cast<Func<Task>>())
        {
            await handler();
        }
    }
}
