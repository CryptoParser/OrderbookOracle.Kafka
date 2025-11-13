using Confluent.Kafka;
using MessagingKafka.Producer.Interfaces;
using MessagingKafka.Settings;
using Microsoft.Extensions.Options;

namespace MessagingKafka.Producer;

public class KafkaProducer<TMessage> : IKafkaProducer<TMessage>
{
    private readonly IProducer<string, TMessage> _producer;
    private readonly string _topic;
    
    public KafkaProducer(IOptions<KafkaSettings> kafkaSettings)
    {
        var config = new ProducerConfig()
        {
            BootstrapServers = kafkaSettings.Value.BootstrapServers
        };

        _producer = new ProducerBuilder<string, TMessage>(config)
            .SetValueSerializer(new KafkaJsonSerializer<TMessage>())
            .Build();

        _topic = kafkaSettings.Value.Topic;
    }

    public async Task ProduceAsync(string key, TMessage message, CancellationToken cancellationToken)
    {
        await _producer.ProduceAsync(_topic, new Message<string, TMessage>()
        {
            Key = key,
            Value = message
        }, cancellationToken);
    }

    public void Dispose()
    {
        _producer?.Dispose();
    }
}