using Confluent.Kafka;
using Microsoft.Extensions.Options;
using PermissionManager.Infrastructure.Kafka;

namespace PermissionManager.API.BackgroundServices;

public class KafkaConsumer(IOptions<KafkaOptions> kafkaOptions, ILogger<KafkaConsumer> logger) : BackgroundService
{
    private readonly IConsumer<Ignore, string> _consumer = new ConsumerBuilder<Ignore, string>(new ConsumerConfig
    {
        BootstrapServers = kafkaOptions.Value.BootstrapServers,
        GroupId = "permissionmanager-consumer-group",
        AutoOffsetReset = AutoOffsetReset.Earliest,
        EnableAutoCommit = true
    }).Build();
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() => StartConsumerLoop(stoppingToken), stoppingToken);
    }

    private void StartConsumerLoop(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(kafkaOptions.Value.TopicName);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = _consumer.Consume(stoppingToken);
                logger.LogInformation("Received event from Kafka topic {TopicName}, message {Message}",
                    consumeResult.Topic, consumeResult.Message.Value);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unable to consume kafka message.");
            }
        }
    } 

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
        _consumer.Close();
        _consumer.Dispose();
        base.Dispose();
    }
}