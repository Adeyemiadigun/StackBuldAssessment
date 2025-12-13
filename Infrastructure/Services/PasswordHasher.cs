using System.Security.Cryptography;
using System.Text;
using Application.Services;

namespace Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    private const int KeySize = 64;
    private const int Iterations = 350000;
    private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA512;

    public (string hashedPassword, string salt) HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be null or empty", nameof(password));

        // Generate a random salt
        var salt = RandomNumberGenerator.GetBytes(KeySize);

        // Hash the password with PBKDF2
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            Iterations,
            HashAlgorithm,
            KeySize);

        return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
    }

    public bool VerifyPassword(string password, string hashedPassword, string salt)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        if (string.IsNullOrWhiteSpace(hashedPassword) || string.IsNullOrWhiteSpace(salt))
            return false;

        try
        {
            var saltBytes = Convert.FromBase64String(salt);
            var hashBytes = Convert.FromBase64String(hashedPassword);

            // Hash the provided password with the same salt
            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                saltBytes,
                Iterations,
                HashAlgorithm,
                KeySize);

            // Compare the hashes using constant-time comparison to prevent timing attacks
            return CryptographicOperations.FixedTimeEquals(hashToCompare, hashBytes);
        }
        catch
        {
            return false;
        }
    }
}