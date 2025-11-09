namespace MessagingKafka;

public interface IMessageHandler<TMessage>
{
    Task HandlerAsync(TMessage message, CancellationToken cancellationToken);
}