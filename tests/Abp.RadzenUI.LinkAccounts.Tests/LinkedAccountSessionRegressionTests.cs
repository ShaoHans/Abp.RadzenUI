using System.Security.Claims;
using Abp.RadzenUI.LinkAccounts.Dtos;
using Microsoft.Extensions.Options;
using Moq;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Users;
using Xunit;

namespace Abp.RadzenUI.LinkAccounts.Tests;

public class LinkedAccountSessionRegressionTests
{
    [Fact]
    public void ApplySwitch_ShouldTrimStack_WhenSwitchingBackToExistingAccount()
    {
        var originalUserId = Guid.NewGuid();
        var originalTenantId = Guid.NewGuid();
        var switchedUserId = Guid.NewGuid();
        var switchedTenantId = Guid.NewGuid();

        var session = new LinkedAccountSessionCacheItem
        {
            SessionId = "session-1",
            Stack =
            [
                NewUser(originalUserId, originalTenantId, "XinLing", "linkx"),
                NewUser(switchedUserId, switchedTenantId, "test1", "linka")
            ]
        };

        LinkedAccountSessionStackManager.ApplySwitch(
            session,
            NewUser(originalUserId, originalTenantId, "XinLing", "linkx"),
            maxSwitchDepth: 5);

        Assert.Single(session.Stack);
        Assert.Equal(originalUserId, session.Stack[0].UserId);
        Assert.Equal(originalTenantId, session.Stack[0].TenantId);
        Assert.Equal("XinLing", session.Stack[0].TenantName);
        Assert.Equal("linkx", session.Stack[0].UserName);
    }

    [Fact]
    public async Task GetCurrentSessionAsync_ShouldReadOriginAndCurrentFromSessionStack()
    {
        var sessionId = "session-2";
        var flowStateStore = new Mock<ILinkedAccountFlowStateStore>();
        flowStateStore
            .Setup(x => x.GetSessionAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LinkedAccountSessionCacheItem
            {
                SessionId = sessionId,
                Stack =
                [
                    NewUser(Guid.NewGuid(), Guid.NewGuid(), "XinLing", "linkx"),
                    NewUser(Guid.NewGuid(), Guid.NewGuid(), "test1", "linka")
                ]
            });

        var currentUser = new Mock<ICurrentUser>();
        currentUser
            .Setup(x => x.FindClaim(LinkedAccountClaimTypes.SessionId))
            .Returns(new Claim(LinkedAccountClaimTypes.SessionId, sessionId));
        currentUser.SetupGet(x => x.UserName).Returns("wrong-user");
        currentUser.SetupGet(x => x.Id).Returns(Guid.NewGuid());

        var currentTenant = new Mock<ICurrentTenant>();
        currentTenant.SetupGet(x => x.Id).Returns(Guid.NewGuid());
        currentTenant.SetupGet(x => x.Name).Returns("WrongTenant");

        var appService = new LinkedAccountAppService(
            null!,
            null!,
            null!,
            flowStateStore.Object,
            currentUser.Object,
            currentTenant.Object,
            Options.Create(new AbpRadzenUILinkAccountsOptions()));

        var session = await appService.GetCurrentSessionAsync();

        Assert.NotNull(session);
        Assert.Equal("XinLing", session!.OriginTenantName);
        Assert.Equal("linkx", session.OriginUserName);
        Assert.Equal("test1", session.CurrentTenantName);
        Assert.Equal("linka", session.CurrentUserName);
        Assert.True(session.CanGoBack);
    }

    [Fact]
    public void SessionDisplayModel_ShouldUseSharedAccountFormatting()
    {
        var display = LinkedAccountSessionDisplayModel.Create(new LinkedAccountSessionDto
        {
            OriginTenantName = null,
            OriginUserName = "linkx",
            CurrentTenantName = "test1",
            CurrentUserName = "linka",
            CanGoBack = true,
        }, "Host");

        Assert.True(display.HasSession);
        Assert.True(display.CanGoBack);
        Assert.Equal("Host / linkx", display.OriginAccountDisplay);
        Assert.Equal("test1 / linka", display.CurrentAccountDisplay);
    }

    private static LinkedAccountUserInfo NewUser(Guid userId, Guid tenantId, string tenantName, string userName)
    {
        return new LinkedAccountUserInfo
        {
            UserId = userId,
            TenantId = tenantId,
            TenantName = tenantName,
            UserName = userName,
        };
    }
}