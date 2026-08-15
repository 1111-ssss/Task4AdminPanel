using Data.Interfaces.Services;

namespace Data.Services;

public class BCryptPasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        return passwordHash;
    }

    public bool VerifyPassword(string password, string hash)
    {
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, hash);

        return isPasswordValid;
    }
}