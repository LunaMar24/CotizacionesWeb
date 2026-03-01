using CotizacionesWeb.Infrastructure.Security;

namespace CotizacionesWeb.Tests;

public class PasswordHasherTests
{
    private readonly PasswordHasher _hasher = new();

    [Fact]
    public void Hash_Returns_NonEmpty_String()
    {
        var hash = _hasher.Hash("secret123");
        Assert.False(string.IsNullOrEmpty(hash));
    }

    [Fact]
    public void Verify_Returns_True_For_Correct_Password()
    {
        var hash = _hasher.Hash("secret123");
        Assert.True(_hasher.Verify("secret123", hash));
    }

    [Fact]
    public void Verify_Returns_False_For_Wrong_Password()
    {
        var hash = _hasher.Hash("secret123");
        Assert.False(_hasher.Verify("wrongpassword", hash));
    }

    [Fact]
    public void Same_Password_Produces_Different_Hashes()
    {
        var hash1 = _hasher.Hash("secret123");
        var hash2 = _hasher.Hash("secret123");
        Assert.NotEqual(hash1, hash2);
    }
}
