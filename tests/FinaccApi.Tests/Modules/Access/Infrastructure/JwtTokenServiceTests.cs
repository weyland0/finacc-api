using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FinaccApi.Modules.Access.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FinaccApi.Tests.Modules.Access.Infrastructure;

[TestClass]
public sealed class JwtTokenServiceTests
{
    private const string Secret = "test-secret-key-with-enough-length-123456789";
    private const string Issuer = "FinaccApi.Tests";
    private const string Audience = "FinaccApi.Tests.Client";

    private static JwtTokenService CreateService()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Secret"] = Secret,
                ["Jwt:Issuer"] = Issuer,
                ["Jwt:Audience"] = Audience
            })
            .Build();

        return new JwtTokenService(configuration);
    }

    [TestMethod]
    public void GenerateAccessToken_WhenUserIdAndEmailAreValid_ContainsUserGuid()
    {
        var service = CreateService();
        var userId = Guid.CreateVersion7();

        var token = service.GenerateAccessToken(userId, "test@gmail.com");
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.AreEqual(userId.ToString(), jwt.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value);
    }

    [TestMethod]
    public void GenerateAccessToken_WhenUserIdIsEmpty_ThrowsArgumentException()
    {
        var service = CreateService();

        Assert.ThrowsException<ArgumentException>(
            () => service.GenerateAccessToken(Guid.Empty, "test@gmail.com"));
    }

    [TestMethod]
    public void GenerateAccessToken_WhenEmailIsInvalid_ThrowsArgumentException()
    {
        var service = CreateService();

        Assert.ThrowsException<ArgumentException>(
            () => service.GenerateAccessToken(Guid.CreateVersion7(), ""));
    }

    [TestMethod]
    public void GenerateRefreshToken_WhenUserIdAndEmailAreValid_RoundTripsUserGuid()
    {
        var service = CreateService();
        var userId = Guid.CreateVersion7();

        var token = service.GenerateRefreshToken(userId, "test@gmail.com");

        Assert.AreEqual(userId, service.ValidateRefreshToken(token));
    }

    [TestMethod]
    public void GenerateRefreshToken_WhenUserIdIsEmpty_ThrowsArgumentException()
    {
        var service = CreateService();

        Assert.ThrowsException<ArgumentException>(
            () => service.GenerateRefreshToken(Guid.Empty, "test@gmail.com"));
    }

    [TestMethod]
    public void GenerateRefreshToken_WhenEmailIsInvalid_ThrowsArgumentException()
    {
        var service = CreateService();

        Assert.ThrowsException<ArgumentException>(
            () => service.GenerateRefreshToken(Guid.CreateVersion7(), ""));
    }

    [TestMethod]
    public void ValidateRefreshToken_WhenTokenIsInvalid_ReturnsNull()
    {
        var service = CreateService();

        Assert.IsNull(service.ValidateRefreshToken("invalid-token"));
    }

    [TestMethod]
    [DataRow("23")]
    [DataRow("00000000-0000-0000-0000-000000000000")]
    [DataRow("invalid-user-id")]
    public void ValidateRefreshToken_WhenSignedTokenHasInvalidUserId_ReturnsNull(string userId)
    {
        var service = CreateService();
        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: [new Claim(ClaimTypes.NameIdentifier, userId)],
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret)),
                SecurityAlgorithms.HmacSha256));

        Assert.IsNull(service.ValidateRefreshToken(new JwtSecurityTokenHandler().WriteToken(token)));
    }
}
