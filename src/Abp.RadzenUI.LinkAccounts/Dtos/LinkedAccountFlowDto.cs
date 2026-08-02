namespace Abp.RadzenUI.LinkAccounts.Dtos;

public class LinkedAccountFlowDto
{
    public string Token { get; set; } = string.Empty;

    public string LoginUrl { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }
}