namespace MessagingKafka.Producer.Interfaces;

public interface IKafkaProducer<in TMessage> : IDisposable
{
    Task ProduceAsync(string key, TMessage message, CancellationToken cancellationToken);
}