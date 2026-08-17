using Data.Common.Enums;

namespace Web.Interfaces;

public interface IAuthCookieService
{
    Task SignIn(string email, string name, string surname, UserStatus status, bool rememberMe);
    Task SignOut();
}