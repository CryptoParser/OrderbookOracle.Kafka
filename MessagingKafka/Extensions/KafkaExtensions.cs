using MessagingKafka.Consumer;
using MessagingKafka.Consumer.Interfaces;
using MessagingKafka.Producer;
using MessagingKafka.Producer.Interfaces;
using MessagingKafka.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MessagingKafka.Extensions;

public static class KafkaExtensions
{
    public static void AddProducer<TMessage>(this IServiceCollection serviceCollection,
        IConfigurationSection configurationSection) 
    {
        serviceCollection.Configure<KafkaSettings>(configurationSection);
        serviceCollection.AddTransient<IKafkaProducer<TMessage>, KafkaProducer<TMessage>>();
    }

    public static IServiceCollection AddConsumer<TMessage, THandler>(this IServiceCollection serviceCollection,
        IConfigurationSection configurationSection) where THandler : class, IMessageHandler<TMessage>
    {
        serviceCollection.Configure<KafkaSettings>(configurationSection);
        serviceCollection.AddHostedService<KafkaConsumer<TMessage>>();
        serviceCollection.AddTransient<IMessageHandler<TMessage>, THandler>();

        return serviceCollection;
    }
}