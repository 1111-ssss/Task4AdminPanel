using Data.Common.Result;

namespace Business.Interfaces.Account;

public interface IEmailSenderService
{
    Task<Result> SendConfirmationEmail(string email, string link);
}