using System.Security.Claims;
using FinaccApi.Shared.Infrastructure.Auth;
using Microsoft.AspNetCore.Http;

namespace FinaccApi.Tests.Modules.Access.Infrastructure;

[TestClass]
public sealed class CurrentUserWithHttpContextTests
{
    [TestMethod]
    [DataRow(ClaimTypes.NameIdentifier)]
    [DataRow("userId")]
    public void UserId_WhenAuthenticatedWithGuidClaim_ReturnsUserGuid(string claimType)
    {
        var userId = Guid.CreateVersion7();
        var currentUser = CreateUser(userId.ToString(), claimType);

        Assert.AreEqual(userId, currentUser.UserId);
        Assert.AreEqual(userId, currentUser.RequiredUserId);
    }

    [TestMethod]
    [DataRow("23")]
    [DataRow("00000000-0000-0000-0000-000000000000")]
    [DataRow("invalid-user-id")]
    public void UserId_WhenAuthenticatedWithInvalidClaim_ReturnsNullAndRequiredIdThrows(string userId)
    {
        var currentUser = CreateUser(userId);

        Assert.IsNull(currentUser.UserId);
        Assert.ThrowsException<InvalidOperationException>(() => { _ = currentUser.RequiredUserId; });
    }

    [TestMethod]
    public void UserId_WhenUnauthenticatedWithGuidClaim_ReturnsNull()
    {
        var currentUser = CreateUser(Guid.CreateVersion7().ToString(), authenticated: false);

        Assert.IsFalse(currentUser.IsAuthenticated);
        Assert.IsNull(currentUser.UserId);
    }

    [TestMethod]
    public void UserId_WhenUserIdClaimIsMissing_ReturnsNull()
    {
        var currentUser = CreateUser(null);

        Assert.IsNull(currentUser.UserId);
    }

    private static CurrentUserWithHttpContext CreateUser(
        string? userId,
        string claimType = ClaimTypes.NameIdentifier,
        bool authenticated = true)
    {
        Claim[] claims = userId is null ? [] : [new Claim(claimType, userId)];
        var identity = new ClaimsIdentity(claims, authenticated ? "test" : null);
        var accessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };

        return new CurrentUserWithHttpContext(accessor);
    }
}
