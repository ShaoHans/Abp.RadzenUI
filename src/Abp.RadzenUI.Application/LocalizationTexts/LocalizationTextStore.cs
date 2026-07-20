using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.RadzenUI.LocalizationTexts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace Abp.RadzenUI.Application.LocalizationTexts;

/// <summary>
/// Provides database localization overrides to the dynamic localization contributor.
///
/// Layering:
///  - L1: per-node in-memory snapshot, read synchronously by the localization pipeline.
///  - L2: <see cref="IDistributedCache{TCacheItem}"/> (e.g. Redis) fronting the database, shared across nodes.
///
/// Freshness: the writing node invalidates L1/L2 immediately. Other nodes pick up changes via
/// stale-while-revalidate: a stale L1 entry is still served while an async refresh reloads it
/// (bounded by <see cref="RefreshInterval"/>), so localization resolution never blocks on I/O.
/// </summary>
public class LocalizationTextStore : ISingletonDependency
{
    /// <summary>How long an L1 snapshot is served before an async refresh is triggered.</summary>
    public static TimeSpan RefreshInterval { get; set; } = TimeSpan.FromSeconds(30);

    private readonly IDistributedCache<LocalizationTextCacheItem> _cache;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ICurrentTenantAccessor _currentTenantAccessor;
    private readonly ILogger<LocalizationTextStore> _logger;

    private readonly ConcurrentDictionary<string, Snapshot> _l1 = new();
    private readonly ConcurrentDictionary<string, byte> _refreshing = new();

    public LocalizationTextStore(
        IDistributedCache<LocalizationTextCacheItem> cache,
        IServiceScopeFactory serviceScopeFactory,
        ICurrentTenantAccessor currentTenantAccessor,
        ILogger<LocalizationTextStore> logger)
    {
        _cache = cache;
        _serviceScopeFactory = serviceScopeFactory;
        _currentTenantAccessor = currentTenantAccessor;
        _logger = logger;
    }

    public Guid? CurrentTenantId => _currentTenantAccessor.Current?.TenantId;

    /// <summary>
    /// Synchronously returns the current override snapshot for the given tuple, or null when not
    /// yet loaded. Triggers a background refresh when the entry is missing or stale.
    /// </summary>
    public IReadOnlyDictionary<string, string>? PeekOrNull(Guid? tenantId, string resourceName, string cultureName)
    {
        var key = L1Key(tenantId, resourceName, cultureName);

        if (_l1.TryGetValue(key, out var snapshot))
        {
            if (DateTime.UtcNow - snapshot.LoadedAt > RefreshInterval)
            {
                QueueRefresh(tenantId, resourceName, cultureName, key);
            }

            return snapshot.Texts;
        }

        QueueRefresh(tenantId, resourceName, cultureName, key);
        return null;
    }

    /// <summary>Loads and caches the merged override snapshot for the given tuple.</summary>
    public async Task<IReadOnlyDictionary<string, string>> GetAsync(Guid? tenantId, string resourceName, string cultureName)
    {
        var merged = await LoadMergedAsync(tenantId, resourceName, cultureName);
        _l1[L1Key(tenantId, resourceName, cultureName)] = new Snapshot(merged, DateTime.UtcNow);
        return merged;
    }

    /// <summary>
    /// Invalidates caches after a write. Removes the affected L2 entry and clears this node's L1
    /// (cheap; writes are rare). Other nodes converge via stale-while-revalidate.
    /// </summary>
    public async Task InvalidateAsync(Guid? tenantId, string resourceName, string cultureName)
    {
        await _cache.RemoveAsync(L2Key(tenantId, resourceName, cultureName));
        _l1.Clear();
        await GetAsync(tenantId, resourceName, cultureName);
    }

    private void QueueRefresh(Guid? tenantId, string resourceName, string cultureName, string key)
    {
        if (!_refreshing.TryAdd(key, 0))
        {
            return;
        }

        _ = Task.Run(async () =>
        {
            try
            {
                await GetAsync(tenantId, resourceName, cultureName);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
            finally
            {
                _refreshing.TryRemove(key, out _);
            }
        });
    }

    private async Task<IReadOnlyDictionary<string, string>> LoadMergedAsync(Guid? tenantId, string resourceName, string cultureName)
    {
        var host = await GetScopedTextsAsync(null, resourceName, cultureName);

        if (tenantId == null)
        {
            return host;
        }

        var tenant = await GetScopedTextsAsync(tenantId, resourceName, cultureName);
        if (tenant.Count == 0)
        {
            return host;
        }

        var merged = new Dictionary<string, string>(host);
        foreach (var kv in tenant)
        {
            merged[kv.Key] = kv.Value;
        }

        return merged;
    }

    private async Task<Dictionary<string, string>> GetScopedTextsAsync(Guid? tenantId, string resourceName, string cultureName)
    {
        var item = await _cache.GetOrAddAsync(
            L2Key(tenantId, resourceName, cultureName),
            () => QueryDatabaseAsync(tenantId, resourceName, cultureName));

        return item?.Texts ?? [];
    }

    private async Task<LocalizationTextCacheItem> QueryDatabaseAsync(Guid? tenantId, string resourceName, string cultureName)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var uowManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();
        var repository = scope.ServiceProvider.GetRequiredService<IRepository<LocalizationText, Guid>>();
        var currentTenant = scope.ServiceProvider.GetRequiredService<ICurrentTenant>();
        var dataFilter = scope.ServiceProvider.GetRequiredService<IDataFilter>();

        using var uow = uowManager.Begin(requiresNew: true, isTransactional: false);
        using (currentTenant.Change(tenantId))
        using (dataFilter.Disable<IMultiTenant>())
        {
            var texts = await repository.GetListAsync(x =>
                x.ResourceName == resourceName
                && x.CultureName == cultureName
                && x.TenantId == tenantId);

            var item = new LocalizationTextCacheItem
            {
                Texts = texts.ToDictionary(x => x.Key, x => x.Value),
            };

            await uow.CompleteAsync();
            return item;
        }
    }

    private static string L1Key(Guid? tenantId, string resourceName, string cultureName)
        => L2Key(tenantId, resourceName, cultureName);

    private static string L2Key(Guid? tenantId, string resourceName, string cultureName)
        => $"{tenantId?.ToString("N") ?? "host"}:{resourceName}:{cultureName}";

    private sealed record Snapshot(IReadOnlyDictionary<string, string> Texts, DateTime LoadedAt);
}
