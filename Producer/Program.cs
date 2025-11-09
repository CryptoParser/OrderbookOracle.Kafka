using MessagingKafka;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProducer<MexcProvider>(builder.Configuration.GetSection("Kafka:mexc_provider"));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/mexc-provider", async (IKafkaProducer<MexcProvider> kafkaProducer) =>
{
    await kafkaProducer.ProduceAsync(new MexcProvider
    {
        Id = Guid.NewGuid().ToString(),
        Name = "first info"
    }, default);
});

app.Run();