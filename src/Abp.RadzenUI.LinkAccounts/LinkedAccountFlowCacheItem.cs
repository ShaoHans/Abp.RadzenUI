namespace Abp.RadzenUI.LinkAccounts;

public class LinkedAccountFlowCacheItem
{
    public string Token { get; set; } = string.Empty;

    public LinkedAccountFlowType FlowType { get; set; }

    public LinkedAccountUserInfo Source { get; set; } = new();

    public LinkedAccountUserInfo? Target { get; set; }

    public string? ReturnUrl { get; set; }

    public DateTime ExpiresAt { get; set; }

    public bool Consumed { get; set; }
}