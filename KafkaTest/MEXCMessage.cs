using MessagingKafka;
using MessagingKafka.Consumer.Interfaces;

namespace KafkaTest;

public class MEXCMessage(ILogger<MEXCMessage> logger) : IMessageHandler<MEXC>
{
    public Task HandlerAsync(MEXC message, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Сообщение доставлено {message.Id} {message.Name} {message.Description}");
        return Task.CompletedTask;
    }
}