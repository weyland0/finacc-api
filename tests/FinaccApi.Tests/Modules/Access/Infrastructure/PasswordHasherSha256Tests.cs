using FinaccApi.Modules.Access.Infrastructure;

namespace FinaccApi.Tests.Modules.Access.Infrastructure;

[TestClass]
public sealed class PasswordHasherSha256Tests
{
    [TestMethod]
    public void Hash_WhenPasswordIsNotEmpty_ReturnsHash()
    {
        var hasher = new PasswordHasherSha256();

        var hash = hasher.Hash("password");

        Assert.IsFalse(string.IsNullOrWhiteSpace(hash));
    }

    [TestMethod]
    public void Verify_WhenPasswordMatchesHash_ReturnsTrue()
    {
        var hasher = new PasswordHasherSha256();
        var hash = hasher.Hash("password");

        var result = hasher.Verify("password", hash);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Verify_WhenPasswordDoesNotMatchHash_ReturnsFalse()
    {
        var hasher = new PasswordHasherSha256();
        var hash = hasher.Hash("password");

        var result = hasher.Verify("wrong-password", hash);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Hash_WhenPasswordIsEmpty_ThrowsArgumentException()
    {
        var hasher = new PasswordHasherSha256();

        Assert.ThrowsException<ArgumentException>(() => hasher.Hash(""));
    }

    [TestMethod]
    public void Verify_WhenPasswordIsEmptyHashIsNot_ReturnsFalse()
    {
        var hasher = new PasswordHasherSha256();
        var hash = hasher.Hash("password");
        var result = hasher.Verify("", hash);

        Assert.IsFalse(result);
    }

    public void Verify_WhenHashIsEmptyPasswordIsNot_ReturnsFalse()
    {
        var hasher = new PasswordHasherSha256();
        var result = hasher.Verify("password", "");

        Assert.IsFalse(result);
    }

    public void Verify_WhenHashAndPasswordAreEmpty_ReturnsFalse()
    {
        var hasher = new PasswordHasherSha256();
        var result = hasher.Verify("", "");

        Assert.IsFalse(result);
    }
}
