namespace Abp.RadzenUI.Application.Contracts.Settings;

public class UpdateAccountSettingsDto
{
    public bool IsSelfRegistrationEnabled { get; set; }

    public bool EnableLocalLogin { get; set; }
}
