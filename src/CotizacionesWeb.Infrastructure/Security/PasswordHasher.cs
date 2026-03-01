using System.Security.Cryptography;

namespace CotizacionesWeb.Infrastructure.Security;

public class PasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);

        var combined = new byte[SaltSize + HashSize];
        Buffer.BlockCopy(salt, 0, combined, 0, SaltSize);
        Buffer.BlockCopy(hash, 0, combined, SaltSize, HashSize);

        return Convert.ToBase64String(combined);
    }

    public bool Verify(string password, string storedHash)
    {
        var combined = Convert.FromBase64String(storedHash);
        if (combined.Length != SaltSize + HashSize)
            return false;

        var salt = new byte[SaltSize];
        Buffer.BlockCopy(combined, 0, salt, 0, SaltSize);

        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);

        var storedHashBytes = new byte[HashSize];
        Buffer.BlockCopy(combined, SaltSize, storedHashBytes, 0, HashSize);

        return CryptographicOperations.FixedTimeEquals(hash, storedHashBytes);
    }
}
