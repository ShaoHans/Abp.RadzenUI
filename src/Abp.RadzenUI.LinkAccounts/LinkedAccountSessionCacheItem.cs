namespace Abp.RadzenUI.LinkAccounts;

public class LinkedAccountSessionCacheItem
{
    public string SessionId { get; set; } = string.Empty;

    public List<LinkedAccountUserInfo> Stack { get; set; } = [];

    public DateTime ExpiresAt { get; set; }
}