using Business.Contracts.Account;

namespace Business.Interfaces.Account;

public interface IEmailQueue
{
    ValueTask QueueEmailAsync(EmailMessage message, CancellationToken cancellationToken = default);
    ValueTask<EmailMessage> DequeueAsync(CancellationToken cancellationToken);
}