namespace Abp.RadzenUI.Models;

public class ChangePasswordModel
{
    public string CurrentPassword { get; set; } = default!;

    public string NewPassword { get; set; } = default!;

    public string NewPasswordConfirm { get; set; } = default!;

    public bool HideOldPasswordInput { get; set; }

    public void Clear()
    {
        CurrentPassword = string.Empty;
        NewPassword = string.Empty;
        NewPasswordConfirm = string.Empty;
    }
}
