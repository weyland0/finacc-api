using FinaccApi.DataAccess;
using FinaccApi.Modules.Access.Application.Auth;
using FinaccApi.Modules.Access.Application.Auth.Register;
using FinaccApi.Modules.Access.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FinaccApi.Tests.Modules.Access.Application.Auth.Register;

[TestClass]
public sealed class RegisterHandlerTests
{
    [TestMethod]
    public async Task Handle_WhenEmailIsNew_CreatesUserAndReturnsAuthResponse()
    {
        await using var context = CreateContext();

        var handler = new RegisterHandler(
            context,
            new FakePasswordHasher(),
            new FakeJwtTokenService());

        var command = CreateCommand();

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);

        Assert.AreEqual("new@test.com", result.Value.CurrentUser.Email);
        Assert.AreEqual("+79990000000", result.Value.CurrentUser.Phone);
        Assert.AreEqual("John", result.Value.CurrentUser.FirstName);
        Assert.AreEqual("Smith", result.Value.CurrentUser.LastName);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.Value.AccessToken));

        var user = await context.Users.SingleAsync();

        Assert.AreEqual("new@test.com", user.Email);
        Assert.AreEqual("hashed:password123", user.PasswordHash);
    }

    [TestMethod]
    public async Task Handle_WhenEmailAlreadyExists_ReturnsFailure()
    {
        await using var context = CreateContext();

        context.Users.Add(new(
            "new@test.com",
            "+79991111111",
            "existing-hash",
            "Existing",
            "User"));

        await context.SaveChangesAsync();

        var handler = new RegisterHandler(
            context,
            new FakePasswordHasher(),
            new FakeJwtTokenService());

        var result = await handler.Handle(CreateCommand(), CancellationToken.None);

        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual(AuthErrors.EmailAlreadyExists, result.Error);
        Assert.AreEqual(1, await context.Users.CountAsync());
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    private static RegisterCommand CreateCommand()
    {
        return new RegisterCommand
        {
            Email = "new@test.com",
            Phone = "+79990000000",
            FirstName = "John",
            LastName = "Smith",
            Password = "password123"
        };
    }

    private sealed class FakePasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            return $"hashed:{password}";
        }

        public bool Verify(string password, string hash)
        {
            return hash == Hash(password);
        }
    }

    private sealed class FakeJwtTokenService : IJwtTokenService
    {
        public string GenerateAccessToken(int userId, string email)
        {
            return $"access-token:{userId}:{email}";
        }

        public string GenerateRefreshToken(int userId, string email)
        {
            return $"refresh-token:{userId}:{email}";
        }

        public int? ValidateRefreshToken(string refreshToken)
        {
            return null;
        }
    }
}
