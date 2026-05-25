using FinaccApi.Modules.Access.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace FinaccApi.Tests.Modules.Access.Infrastructure;

[TestClass]
public sealed class JwtTokenServiceTests
{
    private static IConfiguration CreateConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Secret"] = "test-secret-key-with-enough-length-123456789",
                ["Jwt:Issuer"] = "FinaccApi.Tests",
                ["Jwt:Audience"] = "FinaccApi.Tests.Client"
            })
            .Build();
    }

    private static JwtTokenService CreateService()
    {
        return new JwtTokenService(CreateConfiguration());
    }

    [TestMethod]
    public void GenerateAccessToken_WhenUserIdAndEmailAreValid_ReturnsToken()
    {
        var service = CreateService();

        var token = service.GenerateAccessToken(23, "test@gmail.com");
        Assert.IsFalse(string.IsNullOrWhiteSpace(token));
    }

    [TestMethod]
    public void GenerateAccessToken_WhenUserIdIsInvalid_ThrowsArgumentException()
    {
       var service = CreateService();

        Assert.ThrowsException<ArgumentException>(
            () => service.GenerateAccessToken(-1, "test@gmail.com")
        );
    }

    [TestMethod]
    public void GenerateAccessToken_WhenEmailIsInvalid_ThrowsArgumentException()
    {
       var service = CreateService();

        Assert.ThrowsException<ArgumentException>(
            () => service.GenerateAccessToken(1, "")
        );
    }

    [TestMethod]
    public void GenerateRefreshToken_WhenUserIdAndEmailAreValid_ReturnsToken()
    {
        var service = CreateService();

        var token = service.GenerateRefreshToken(23, "test@gmail.com");
        Assert.AreEqual(23, service.ValidateRefreshToken(token));
    }

    [TestMethod]
    public void GenerateRefreshToken_WhenUserIdIsInvalid_ThrowsArgumentException()
    {
       var service = CreateService();

        Assert.ThrowsException<ArgumentException>(
            () => service.GenerateRefreshToken(-1, "test@gmail.com")
        );
    }

    [TestMethod]
    public void GenerateRefreshToken_WhenEmailIsInvalid_ThrowsArgumentException()
    {
       var service = CreateService();

        Assert.ThrowsException<ArgumentException>(
            () => service.GenerateRefreshToken(1, "")
        );
    }

    [TestMethod]
    public void ValidateRefreshToken_WhenTokenIsInvalid_ReturnsNull()
    {
        var service = CreateService();

        var userId = service.ValidateRefreshToken("invalid-token");

        Assert.IsNull(userId);
    }

    [TestMethod]
    public void ValidateRefreshToken_WhenTokenIsValid_ReturnsUserUd()
    {
        var service = CreateService();

        var token = service.GenerateRefreshToken(23, "test@gmail.com");
        Assert.AreEqual(23, service.ValidateRefreshToken(token));
    }
}
