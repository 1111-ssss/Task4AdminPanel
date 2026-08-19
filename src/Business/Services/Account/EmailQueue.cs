using System.Threading.Channels;
using Business.Contracts.Account;
using Business.Interfaces.Account;

namespace Business.Services.Account;

public class EmailQueue : IEmailQueue
{
    private readonly Channel<EmailMessage> _queue;

    public EmailQueue(int capacity = 1000)
    {
        var options = new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        _queue = Channel.CreateBounded<EmailMessage>(options);
    }

    public async ValueTask QueueEmailAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        await _queue.Writer.WriteAsync(message, cancellationToken);
    }

    public async ValueTask<EmailMessage> DequeueAsync(CancellationToken cancellationToken)
    {
        return await _queue.Reader.ReadAsync(cancellationToken);
    }
}