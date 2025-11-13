using KafkaTest;
using MessagingKafka;
using MessagingKafka.Extensions;
using MessagingKafka.Producer.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProducer<MEXC>(builder.Configuration.GetSection("Kafka:Producers:MEXC"));
builder.Services.AddConsumer<MEXC, MEXCMessage>(builder.Configuration.GetSection("Kafka:Consumers:MEXC"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/create-mexc", async (IKafkaProducer<MEXC> kafkaProducer) =>
{
    await kafkaProducer.ProduceAsync("sjvnjsbv43rfvdvb", new MEXC
    {
        Id = 1,
        Name = "Hello",
        Description = "vhsdvcisdjbvayrafvbawiebeyveywva"
    }, default);
});

app.Run();