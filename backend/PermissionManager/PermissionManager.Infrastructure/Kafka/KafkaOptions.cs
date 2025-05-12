namespace PermissionManager.Infrastructure.Kafka;

public class KafkaOptions
{
    public required string BootstrapServers { get; init; }
    
    public required string TopicName { get; init; }
}