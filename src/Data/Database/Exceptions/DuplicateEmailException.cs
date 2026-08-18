namespace Data.Database.Exceptions;

public class DuplicateEmailException : Exception
{
    public string Email { get; }

    public DuplicateEmailException(string email)
        : base($"User with email '{email}' already exists.")
    {
        Email = email;
    }

    public DuplicateEmailException(string email, Exception innerException)
        : base($"User with email '{email}' already exists.", innerException)
    {
        Email = email;
    }
}