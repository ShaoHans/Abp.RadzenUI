namespace Abp.RadzenUI.LinkAccounts;

public class LinkedAccountUserInfo
{
    public Guid UserId { get; set; }

    public Guid? TenantId { get; set; }

    public string? UserName { get; set; }

    public string? TenantName { get; set; }
}