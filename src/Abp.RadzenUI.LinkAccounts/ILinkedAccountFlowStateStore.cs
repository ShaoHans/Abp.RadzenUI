namespace Abp.RadzenUI.LinkAccounts;

public interface ILinkedAccountFlowStateStore
{
    Task<LinkedAccountFlowCacheItem> CreateAsync(
        LinkedAccountFlowType flowType,
        LinkedAccountUserInfo source,
        LinkedAccountUserInfo? target = null,
        string? returnUrl = null,
        CancellationToken cancellationToken = default);

    Task<LinkedAccountFlowCacheItem?> GetAsync(string token, CancellationToken cancellationToken = default);

    Task<LinkedAccountFlowCacheItem?> ConsumeAsync(string token, CancellationToken cancellationToken = default);

    Task<bool> MarkConsumedAsync(string token, CancellationToken cancellationToken = default);

    Task<LinkedAccountSessionCacheItem?> GetSessionAsync(string sessionId, CancellationToken cancellationToken = default);

    Task SetSessionAsync(LinkedAccountSessionCacheItem session, CancellationToken cancellationToken = default);

    Task RemoveSessionAsync(string sessionId, CancellationToken cancellationToken = default);
}