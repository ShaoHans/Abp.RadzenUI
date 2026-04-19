using System.Security.Claims;
using Abp.RadzenUI.LinkAccounts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;

namespace Abp.RadzenUI.Services;

public class LinkedAccountSignInManager(
    SignInManager<Volo.Abp.Identity.IdentityUser> signInManager,
    IdentityUserManager userManager,
    IdentityLinkUserManager identityLinkUserManager,
    IdentityDynamicClaimsPrincipalContributorCache identityDynamicClaimsPrincipalContributorCache,
    ILinkedAccountFlowStateStore flowStateStore,
    IHttpContextAccessor httpContextAccessor,
    ICurrentTenant currentTenant,
    IOptions<AbpAspNetCoreMultiTenancyOptions> multiTenancyOptions,
    IOptions<AbpRadzenUILinkAccountsOptions> linkedAccountOptions)
{
    public async Task<string> BeginLinkLoginAsync(string token, CancellationToken cancellationToken = default)
    {
        var flow = await flowStateStore.GetAsync(token, cancellationToken)
            ?? throw new InvalidOperationException("Linked account flow was not found.");

        if (flow.FlowType != LinkedAccountFlowType.LinkLogin)
        {
            throw new InvalidOperationException("Invalid linked account flow type.");
        }

        await signInManager.SignOutAsync();
        var httpContext = GetHttpContext();
        await httpContext.SignOutAsync();

        return $"{linkedAccountOptions.Value.LoginRoute}?skipAutoLogin=true&linkToken={Uri.EscapeDataString(token)}";
    }

    public async Task<string> CompleteLinkLoginAsync(string token, Volo.Abp.Identity.IdentityUser targetUser, bool isPersistent, CancellationToken cancellationToken = default)
    {
        var flow = await flowStateStore.GetAsync(token, cancellationToken)
            ?? throw new InvalidOperationException("Linked account flow was expired or already consumed.");

        if (flow.Consumed || flow.ExpiresAt <= DateTime.UtcNow)
        {
            throw new InvalidOperationException("Linked account flow was expired or already consumed.");
        }

        if (flow.FlowType != LinkedAccountFlowType.LinkLogin)
        {
            throw new InvalidOperationException("Invalid linked account flow type.");
        }

        var source = new IdentityLinkUserInfo(flow.Source.UserId, flow.Source.TenantId);
        var target = new IdentityLinkUserInfo(targetUser.Id, targetUser.TenantId);
        if (!await identityLinkUserManager.IsLinkedAsync(source, target, false, cancellationToken))
        {
            await identityLinkUserManager.LinkAsync(source, target, cancellationToken);
        }

        var session = new LinkedAccountSessionCacheItem
        {
            SessionId = Guid.NewGuid().ToString("N"),
            Stack =
            [
                flow.Source,
                new LinkedAccountUserInfo
                {
                    UserId = targetUser.Id,
                    TenantId = targetUser.TenantId,
                    UserName = targetUser.UserName,
                    TenantName = currentTenant.Name,
                }
            ],
            ExpiresAt = DateTime.UtcNow.AddHours(linkedAccountOptions.Value.SessionLifetimeHours)
        };

        await flowStateStore.SetSessionAsync(session, cancellationToken);
        await SignInAsync(targetUser, isPersistent, session);

        if (!await flowStateStore.MarkConsumedAsync(token, cancellationToken))
        {
            throw new InvalidOperationException("Linked account flow was expired or already consumed.");
        }

        return $"{linkedAccountOptions.Value.CallbackRoute}?returnUrl={Uri.EscapeDataString(flow.ReturnUrl ?? "/")}";
    }

    public async Task<string> SwitchAsync(string token, CancellationToken cancellationToken = default)
    {
        var flow = await flowStateStore.ConsumeAsync(token, cancellationToken)
            ?? throw new InvalidOperationException("Linked account flow was expired or already consumed.");

        if (flow.FlowType != LinkedAccountFlowType.Switch || flow.Target == null)
        {
            throw new InvalidOperationException("Invalid linked account flow type.");
        }

        var targetUser = await GetUserAsync(flow.Target.UserId, flow.Target.TenantId);
        var session = await GetOrCreateSessionAsync(flow.Source, cancellationToken);

        LinkedAccountSessionStackManager.ApplySwitch(
            session,
            new LinkedAccountUserInfo
            {
                UserId = targetUser.Id,
                TenantId = targetUser.TenantId,
                UserName = targetUser.UserName,
                TenantName = flow.Target.TenantName,
            },
            linkedAccountOptions.Value.MaxSwitchDepth);

        if (session.Stack.Count <= 1)
        {
            await flowStateStore.RemoveSessionAsync(session.SessionId, cancellationToken);
            await SignInAsync(targetUser, true, null);
        }
        else
        {
            session.ExpiresAt = DateTime.UtcNow.AddHours(linkedAccountOptions.Value.SessionLifetimeHours);
            await flowStateStore.SetSessionAsync(session, cancellationToken);
            await SignInAsync(targetUser, true, session);
        }

        return flow.ReturnUrl ?? "/";
    }

    public async Task<string> BackAsync(string? returnUrl = null, CancellationToken cancellationToken = default)
    {
        var sessionId = GetHttpContext().User.FindFirstValue(LinkedAccountClaimTypes.SessionId);
        if (sessionId.IsNullOrWhiteSpace())
        {
            return returnUrl ?? "/";
        }

        var session = await flowStateStore.GetSessionAsync(sessionId!, cancellationToken);
        if (session == null || session.Stack.Count <= 1)
        {
            await flowStateStore.RemoveSessionAsync(sessionId!, cancellationToken);
            return returnUrl ?? "/";
        }

        session.Stack.RemoveAt(session.Stack.Count - 1);
        Volo.Abp.Identity.IdentityUser previousUser;
        try
        {
            var previous = session.Stack.Last();
            previousUser = await GetUserAsync(previous.UserId, previous.TenantId);
        }
        catch (EntityNotFoundException)
        {
            await flowStateStore.RemoveSessionAsync(session.SessionId, cancellationToken);
            return returnUrl ?? "/";
        }

        if (session.Stack.Count == 1)
        {
            await flowStateStore.RemoveSessionAsync(session.SessionId, cancellationToken);
            await SignInAsync(previousUser, true, null);
        }
        else
        {
            session.ExpiresAt = DateTime.UtcNow.AddHours(linkedAccountOptions.Value.SessionLifetimeHours);
            await flowStateStore.SetSessionAsync(session, cancellationToken);
            await SignInAsync(previousUser, true, session);
        }

        return returnUrl ?? "/";
    }

    private async Task<LinkedAccountSessionCacheItem> GetOrCreateSessionAsync(LinkedAccountUserInfo source, CancellationToken cancellationToken)
    {
        var sessionId = GetHttpContext().User.FindFirstValue(LinkedAccountClaimTypes.SessionId);
        if (!sessionId.IsNullOrWhiteSpace())
        {
            var currentSession = await flowStateStore.GetSessionAsync(sessionId!, cancellationToken);
            if (currentSession != null)
            {
                return currentSession;
            }
        }

        return new LinkedAccountSessionCacheItem
        {
            SessionId = Guid.NewGuid().ToString("N"),
            Stack = [source],
            ExpiresAt = DateTime.UtcNow.AddHours(linkedAccountOptions.Value.SessionLifetimeHours)
        };
    }

    private async Task SignInAsync(Volo.Abp.Identity.IdentityUser user, bool isPersistent, LinkedAccountSessionCacheItem? session)
    {
        var httpContext = GetHttpContext();
        AbpMultiTenancyCookieHelper.SetTenantCookie(httpContext, user.TenantId, multiTenancyOptions.Value.TenantKey);

        await signInManager.SignOutAsync();
        await httpContext.SignOutAsync();

        var claims = session == null || session.Stack.Count <= 1
            ? Enumerable.Empty<Claim>()
            :
            [
                new Claim(LinkedAccountClaimTypes.SessionId, session.SessionId),
                new Claim(LinkedAccountClaimTypes.OriginUserId, session.Stack.First().UserId.ToString("D")),
                new Claim(LinkedAccountClaimTypes.OriginTenantId, session.Stack.First().TenantId?.ToString("D") ?? string.Empty),
                new Claim(LinkedAccountClaimTypes.CurrentUserId, user.Id.ToString("D")),
                new Claim(LinkedAccountClaimTypes.CurrentTenantId, user.TenantId?.ToString("D") ?? string.Empty),
            ];

        await signInManager.SignInWithClaimsAsync(user, isPersistent, claims);
        await identityDynamicClaimsPrincipalContributorCache.ClearAsync(user.Id, user.TenantId);
    }

    private async Task<Volo.Abp.Identity.IdentityUser> GetUserAsync(Guid userId, Guid? tenantId)
    {
        using (currentTenant.Change(tenantId))
        {
            return await userManager.GetByIdAsync(userId);
        }
    }

    private HttpContext GetHttpContext()
    {
        return httpContextAccessor.HttpContext ?? throw new InvalidOperationException("Current HttpContext is not available.");
    }
}