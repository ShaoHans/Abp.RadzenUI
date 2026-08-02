namespace Abp.RadzenUI.Models;

public class DialogFromOption<TCreateOrUpdateInput> where TCreateOrUpdateInput : class, new()
{
    public TCreateOrUpdateInput Model { get; set; } = new();

    public Func<TCreateOrUpdateInput, Task> OnSubmit { get; set; } = default!;

    public Action OnCancel { get; set; } = default!;

    public bool IsSaving { get; private set; }

    public async Task SubmitAsync(
        TCreateOrUpdateInput model,
        Func<Task>? stateHasChangedAsync = null
    )
    {
        if (IsSaving)
        {
            return;
        }

        IsSaving = true;
        await NotifyStateChangedAsync(stateHasChangedAsync);

        try
        {
            await OnSubmit(model);
        }
        finally
        {
            IsSaving = false;
            await NotifyStateChangedAsync(stateHasChangedAsync);
        }
    }

    static Task NotifyStateChangedAsync(Func<Task>? stateHasChangedAsync)
    {
        return stateHasChangedAsync is null ? Task.CompletedTask : stateHasChangedAsync();
    }
}
