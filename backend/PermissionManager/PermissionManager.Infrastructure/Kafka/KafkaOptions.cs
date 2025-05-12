namespace PermissionManager.Infrastructure.Kafka;

/// <summary>
/// Represents configuration options for connecting to Kafka.
/// </summary>
public class KafkaOptions
{
    /// <summary>
    /// The Kafka bootstrap servers.
    /// </summary>
    public required string BootstrapServers { get; init; }
    
    /// <summary>
    /// The Kafka topic name for publishing messages.
    /// </summary>
    public required string TopicName { get; init; }
}