namespace MessagingKafka.Consumer.Interfaces;

public interface IMessageHandler<in TMessage>
{
    Task HandlerAsync(TMessage message, CancellationToken cancellationToken);
}