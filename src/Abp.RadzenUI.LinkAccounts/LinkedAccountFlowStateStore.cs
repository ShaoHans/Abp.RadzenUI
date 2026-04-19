using Volo.Abp.Caching;
using Volo.Abp.MultiTenancy;

namespace Abp.RadzenUI.LinkAccounts;

public class LinkedAccountFlowStateStore(
    IDistributedCache<LinkedAccountFlowCacheItem> flowCache,
    IDistributedCache<LinkedAccountSessionCacheItem> sessionCache,
    ICurrentTenant currentTenant,
    Microsoft.Extensions.Options.IOptions<AbpRadzenUILinkAccountsOptions> options)
    : ILinkedAccountFlowStateStore
{
    public async Task<LinkedAccountFlowCacheItem> CreateAsync(
        LinkedAccountFlowType flowType,
        LinkedAccountUserInfo source,
        LinkedAccountUserInfo? target = null,
        string? returnUrl = null,
        CancellationToken cancellationToken = default)
    {
        var token = Guid.NewGuid().ToString("N");
        var item = new LinkedAccountFlowCacheItem
        {
            Token = token,
            FlowType = flowType,
            Source = source,
            Target = target,
            ReturnUrl = returnUrl,
            ExpiresAt = DateTime.UtcNow.AddSeconds(options.Value.FlowTokenLifetimeSeconds)
        };

        using (currentTenant.Change(null))
        {
            await flowCache.SetAsync(
                GetFlowCacheKey(token),
                item,
                new Microsoft.Extensions.Caching.Distributed.DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(options.Value.FlowTokenLifetimeSeconds)
                },
                token: cancellationToken);
        }

        return item;
    }

    public async Task<LinkedAccountFlowCacheItem?> GetAsync(string token, CancellationToken cancellationToken = default)
    {
        using (currentTenant.Change(null))
        {
            return await flowCache.GetAsync(GetFlowCacheKey(token), token: cancellationToken);
        }
    }

    public async Task<LinkedAccountFlowCacheItem?> ConsumeAsync(string token, CancellationToken cancellationToken = default)
    {
        var item = await GetAsync(token, cancellationToken);
        if (item == null || item.Consumed || item.ExpiresAt <= DateTime.UtcNow)
        {
            return null;
        }

        item.Consumed = true;
        await flowCache.SetAsync(
            GetFlowCacheKey(token),
            item,
            new Microsoft.Extensions.Caching.Distributed.DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            },
            token: cancellationToken);

        return item;
    }

    public async Task<bool> MarkConsumedAsync(string token, CancellationToken cancellationToken = default)
    {
        var item = await GetAsync(token, cancellationToken);
        if (item == null || item.Consumed || item.ExpiresAt <= DateTime.UtcNow)
        {
            return false;
        }

        item.Consumed = true;
        await flowCache.SetAsync(
            GetFlowCacheKey(token),
            item,
            new Microsoft.Extensions.Caching.Distributed.DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            },
            token: cancellationToken);

        return true;
    }

    public async Task<LinkedAccountSessionCacheItem?> GetSessionAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        using (currentTenant.Change(null))
        {
            return await sessionCache.GetAsync(GetSessionCacheKey(sessionId), token: cancellationToken);
        }
    }

    public async Task SetSessionAsync(LinkedAccountSessionCacheItem session, CancellationToken cancellationToken = default)
    {
        using (currentTenant.Change(null))
        {
            await sessionCache.SetAsync(
                GetSessionCacheKey(session.SessionId),
                session,
                new Microsoft.Extensions.Caching.Distributed.DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(options.Value.SessionLifetimeHours)
                },
                token: cancellationToken);
        }
    }

    public async Task RemoveSessionAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        using (currentTenant.Change(null))
        {
            await sessionCache.RemoveAsync(GetSessionCacheKey(sessionId), token: cancellationToken);
        }
    }

    private static string GetFlowCacheKey(string token)
    {
        return LinkedAccountConsts.LinkFlowCacheKeyPrefix + token;
    }

    private static string GetSessionCacheKey(string sessionId)
    {
        return LinkedAccountConsts.SessionCacheKeyPrefix + sessionId;
    }
}