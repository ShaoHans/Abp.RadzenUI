using Abp.RadzenUI.LinkAccounts.Dtos;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;
using Volo.Abp.Users;

namespace Abp.RadzenUI.LinkAccounts;

public class LinkedAccountAppService(
    IdentityLinkUserManager identityLinkUserManager,
    IIdentityUserRepository userRepository,
    ITenantRepository tenantRepository,
    ILinkedAccountFlowStateStore flowStateStore,
    ICurrentUser currentUser,
    ICurrentTenant currentTenant,
    Microsoft.Extensions.Options.IOptions<AbpRadzenUILinkAccountsOptions> options)
    : ApplicationService, ILinkedAccountAppService
{
    public async Task<IReadOnlyList<LinkedAccountDto>> GetListAsync(CancellationToken cancellationToken = default)
    {
        var current = GetCurrentLinkUserInfo();
        var links = await identityLinkUserManager.GetListAsync(current, true, cancellationToken: cancellationToken);
        if (links.Count == 0)
        {
            return [];
        }

        var directKeys = new HashSet<string>(
            (await identityLinkUserManager.GetListAsync(current, false, cancellationToken: cancellationToken))
            .SelectMany(link => GetCandidates(link))
            .Select(x => GetKey(x.UserId, x.TenantId)));

        var linkedUsers = links
            .SelectMany(link => GetCandidates(link))
            .Where(user => user.UserId != current.UserId || user.TenantId != current.TenantId)
            .DistinctBy(x => $"{x.TenantId:N}:{x.UserId:N}")
            .ToList();

        var tenantNameMap = await GetTenantNameMapAsync(linkedUsers, cancellationToken);
        var results = new List<LinkedAccountDto>(linkedUsers.Count);
        foreach (var linkedUser in linkedUsers)
        {
            using (CurrentTenant.Change(linkedUser.TenantId))
            {
                var user = await userRepository.GetAsync(linkedUser.UserId, cancellationToken: cancellationToken);
                var key = GetKey(linkedUser.UserId, linkedUser.TenantId);
                results.Add(new LinkedAccountDto
                {
                    UserId = linkedUser.UserId,
                    TenantId = linkedUser.TenantId,
                    UserName = user.UserName,
                    TenantName = tenantNameMap.GetValueOrDefault(GetTenantKey(linkedUser.TenantId)),
                    IsDirectLink = directKeys.Contains(key),
                    CanBeUnlinked = directKeys.Contains(key),
                    IsCurrent = false,
                });
            }
        }

        return results.OrderBy(x => x.TenantName).ThenBy(x => x.UserName).ToList();
    }

    public async Task<LinkedAccountSessionDto?> GetCurrentSessionAsync(CancellationToken cancellationToken = default)
    {
        var sessionId = currentUser.FindClaim(LinkedAccountClaimTypes.SessionId)?.Value;
        if (sessionId.IsNullOrWhiteSpace())
        {
            return null;
        }

        var session = await flowStateStore.GetSessionAsync(sessionId, cancellationToken);
        if (session == null || session.Stack.Count == 0)
        {
            return null;
        }

        var origin = session.Stack.First();
        var current = session.Stack.Last();
        return new LinkedAccountSessionDto
        {
            SessionId = session.SessionId,
            OriginTenantId = origin.TenantId,
            OriginTenantName = origin.TenantName,
            OriginUserId = origin.UserId,
            OriginUserName = origin.UserName,
            CurrentTenantId = current.TenantId,
            CurrentTenantName = current.TenantName,
            CurrentUserId = current.UserId,
            CurrentUserName = current.UserName,
            CanGoBack = session.Stack.Count > 1,
        };
    }

    public async Task<LinkedAccountFlowDto> CreateLinkLoginAsync(CreateLinkedAccountLinkDto input, CancellationToken cancellationToken = default)
    {
        var flow = await flowStateStore.CreateAsync(
            LinkedAccountFlowType.LinkLogin,
            await GetCurrentUserInfoAsync(cancellationToken),
            returnUrl: input.ReturnUrl,
            cancellationToken: cancellationToken);

        return new LinkedAccountFlowDto
        {
            Token = flow.Token,
            LoginUrl = $"{options.Value.LinkLoginStartRoute}?token={flow.Token}",
            ExpiresAt = flow.ExpiresAt,
        };
    }

    public async Task<LinkedAccountFlowDto> CreateSwitchAsync(LinkedAccountSwitchDto input, CancellationToken cancellationToken = default)
    {
        var current = GetCurrentLinkUserInfo();
        var target = new IdentityLinkUserInfo(input.UserId, input.TenantId);
        if (!await identityLinkUserManager.IsLinkedAsync(current, target, true, cancellationToken))
        {
            throw new BusinessException("AbpRadzenUI:LinkedAccountNotFound");
        }

        var sourceUser = await GetCurrentUserInfoAsync(cancellationToken);

        using (CurrentTenant.Change(input.TenantId))
        {
            var targetUser = await userRepository.GetAsync(input.UserId, cancellationToken: cancellationToken);
            var flow = await flowStateStore.CreateAsync(
                LinkedAccountFlowType.Switch,
                sourceUser,
                new LinkedAccountUserInfo
                {
                    UserId = targetUser.Id,
                    TenantId = targetUser.TenantId,
                    UserName = targetUser.UserName,
                    TenantName = (await GetTenantNameMapAsync([new IdentityLinkUserInfo(targetUser.Id, targetUser.TenantId)], cancellationToken)).GetValueOrDefault(GetTenantKey(targetUser.TenantId)),
                },
                input.ReturnUrl,
                cancellationToken);

            return new LinkedAccountFlowDto
            {
                Token = flow.Token,
                LoginUrl = $"{options.Value.SwitchRoute}?token={flow.Token}",
                ExpiresAt = flow.ExpiresAt,
            };
        }
    }

    public async Task DeleteAsync(LinkedAccountSwitchDto input, CancellationToken cancellationToken = default)
    {
        var current = GetCurrentLinkUserInfo();
        await identityLinkUserManager.UnlinkAsync(current, new IdentityLinkUserInfo(input.UserId, input.TenantId), cancellationToken);
    }

    private IdentityLinkUserInfo GetCurrentLinkUserInfo()
    {
        if (!currentUser.Id.HasValue)
        {
            throw new AbpAuthorizationException();
        }

        return new IdentityLinkUserInfo(currentUser.Id.Value, currentTenant.Id);
    }

    private static IEnumerable<IdentityLinkUserInfo> GetCandidates(IdentityLinkUser link)
    {
        yield return new IdentityLinkUserInfo(link.SourceUserId, link.SourceTenantId);
        yield return new IdentityLinkUserInfo(link.TargetUserId, link.TargetTenantId);
    }

    private static string GetKey(Guid userId, Guid? tenantId)
    {
        return $"{tenantId:N}:{userId:N}";
    }

    private async Task<LinkedAccountUserInfo> GetCurrentUserInfoAsync(CancellationToken cancellationToken)
    {
        return new LinkedAccountUserInfo
        {
            UserId = currentUser.GetId(),
            TenantId = currentTenant.Id,
            UserName = currentUser.UserName,
            TenantName = (await GetTenantNameMapAsync([new IdentityLinkUserInfo(currentUser.GetId(), currentTenant.Id)], cancellationToken))
                .GetValueOrDefault(GetTenantKey(currentTenant.Id)),
        };
    }

    private async Task<Dictionary<string, string?>> GetTenantNameMapAsync(
        IReadOnlyCollection<IdentityLinkUserInfo> linkedUsers,
        CancellationToken cancellationToken)
    {
        var tenantIds = linkedUsers.Where(x => x.TenantId.HasValue).Select(x => x.TenantId!.Value).Distinct().ToList();
        var tenants = tenantIds.Count == 0
            ? []
            : (await tenantRepository.GetListAsync(cancellationToken: cancellationToken))
                .Where(x => tenantIds.Contains(x.Id))
                .ToList();

        var map = tenants.ToDictionary(x => GetTenantKey(x.Id), x => (string?)x.Name);
        map.TryAdd(GetTenantKey(null), null);
        return map;
    }

    private static string GetTenantKey(Guid? tenantId)
    {
        return tenantId?.ToString("N") ?? "host";
    }
}