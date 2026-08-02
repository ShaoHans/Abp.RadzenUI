namespace Abp.RadzenUI.LinkAccounts.Dtos;

public class LinkedAccountSwitchDto
{
    public Guid UserId { get; set; }

    public Guid? TenantId { get; set; }

    public string? ReturnUrl { get; set; }
}