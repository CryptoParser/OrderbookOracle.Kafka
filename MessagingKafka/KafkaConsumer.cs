using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace MessagingKafka;

public class KafkaConsumer<TMessage> : BackgroundService
{
    private readonly IConsumer<string, TMessage> _consumer;
    private readonly string _topic;
    private readonly IMessageHandler<TMessage> _messageHandler;

    public KafkaConsumer(IOptions<KafkaSettings> kafkaSettings, IMessageHandler<TMessage> messageHandler)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = kafkaSettings.Value.BootstrapServers,
            GroupId = kafkaSettings.Value.GroupId
        };

        _topic = kafkaSettings.Value.Topic;
        _messageHandler = messageHandler;
        
        _consumer = new ConsumerBuilder<string, TMessage>(config)
            .SetValueDeserializer(new KafkaValueDeserializer<TMessage>())
            .Build();
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() => ConsumeAsync(stoppingToken), stoppingToken);
    }

    private async Task? ConsumeAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(_topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var result = _consumer.Consume(stoppingToken);
                await _messageHandler.HandlerAsync(result.Message.Value, stoppingToken);
            }
        }
        catch (ConsumeException ex) when (ex.Error.IsFatal)
        {
            Console.WriteLine($"Фатальная ошибка Kafka: {ex.Error.Reason}");
            throw;
        }
        catch (ConsumeException ex)
        {
            Console.WriteLine($"Ошибка Kafka: {ex.Error.Reason}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка обработки сообщения: {ex.Message}");
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _consumer.Close();
        return base.StopAsync(cancellationToken);
    }
}