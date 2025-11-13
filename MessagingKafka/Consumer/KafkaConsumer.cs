using Confluent.Kafka;
using MessagingKafka.Consumer.Deserializer;
using MessagingKafka.Consumer.Interfaces;
using MessagingKafka.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MessagingKafka.Consumer;

public class KafkaConsumer<TMessage> : BackgroundService
{
    private readonly IConsumer<string, TMessage> _consumer;
    private readonly string _topic;
    private readonly IMessageHandler<TMessage> _messageHandler;
    private readonly ILogger<KafkaConsumer<TMessage>> _logger;

    public KafkaConsumer(IOptions<KafkaSettings> kafkaSettings, IMessageHandler<TMessage> messageHandler,  ILogger<KafkaConsumer<TMessage>> logger)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = kafkaSettings.Value.BootstrapServers,
            GroupId = kafkaSettings.Value.GroupId
        };

        _topic = kafkaSettings.Value.Topic;
        _messageHandler = messageHandler;
        _logger = logger;
        
        _consumer = new ConsumerBuilder<string, TMessage>(config)
            .SetValueDeserializer(new KafkaValueDeserializer<TMessage>())
            .Build();
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ConsumeAsync(stoppingToken);
    }

    private async Task ConsumeAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();
        
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
        }
        catch (ConsumeException ex)
        {
            Console.WriteLine($"Ошибка Kafka: {ex.Error.Reason}");
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Consumer остановлен по cancellation token");
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex, "Operation cancelled (timeout?)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Неожиданная ошибка: {ex.Message}");
        }
        finally
        {
            _consumer.Close();
        }
    }
}