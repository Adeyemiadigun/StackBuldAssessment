namespace Application.Services;

public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a password using a secure algorithm
    /// </summary>
    /// <param name="password">The plain text password</param>
    /// <returns>A tuple containing the hashed password and the salt</returns>
    (string hashedPassword, string salt) HashPassword(string password);

    /// <summary>
    /// Verifies a password against a hash
    /// </summary>
    /// <param name="password">The plain text password to verify</param>
    /// <param name="hashedPassword">The stored hash</param>
    /// <param name="salt">The salt used to hash the password</param>
    /// <returns>True if the password matches, false otherwise</returns>
    bool VerifyPassword(string password, string hashedPassword, string salt);
}