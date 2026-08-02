namespace Abp.RadzenUI.LinkAccounts.Dtos;

public class LinkedAccountDto
{
    public Guid? TenantId { get; set; }

    public string? TenantName { get; set; }

    public Guid UserId { get; set; }

    public string UserName { get; set; } = string.Empty;

    public bool IsDirectLink { get; set; }

    public bool IsCurrent { get; set; }

    public bool CanBeUnlinked { get; set; }
}