namespace Abp.RadzenUI;

public sealed class MessageCenterState
{
    public event Func<Task>? Changed;

    public async Task NotifyChangedAsync()
    {
        if (Changed is null)
        {
            return;
        }

        foreach (var handler in Changed.GetInvocationList().Cast<Func<Task>>())
        {
            await handler();
        }
    }
}