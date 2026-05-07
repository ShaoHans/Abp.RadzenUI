namespace Abp.RadzenUI;

public class SideDialogState<T>
    where T : class
{
    private T? _selected;

    public T? Selected => _selected;

    public event Func<T?, Task>? OnChanged;

    public async Task SetSelectedAsync(T? value)
    {
        _selected = value;

        if (OnChanged != null)
        {
            await OnChanged.Invoke(value);
        }
    }

    public Task NotifyCloseAsync()
    {
        return SetSelectedAsync(null);
    }
}
