using System.Text.Json;
using System.Text.Json.Serialization;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PermissionManager.Application.DTOs;
using PermissionManager.Application.Interfaces;

namespace PermissionManager.Infrastructure.Kafka;

/// <summary>
/// Provides Kafka-based message production for operations.
/// </summary>
public class KafkaProducerService(IOptions<KafkaOptions> kafkaOptions, ILogger<KafkaProducerService> logger) : IProducerService, IDisposable
{
    private readonly JsonSerializerOptions _jsonSerializerOptions =
        new(JsonSerializerDefaults.General)
        {
            Converters = { new JsonStringEnumConverter() },
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };
    private readonly KafkaOptions _kafkaOptions = kafkaOptions.Value;
    private readonly IProducer<Null, string> _kafkaProducer = new ProducerBuilder<Null, string>(new ProducerConfig
    {
        BootstrapServers = kafkaOptions.Value.BootstrapServers,
        MessageTimeoutMs = 5000,
        RequestTimeoutMs = 3000,
        Acks = Acks.Leader,
        RetryBackoffMs = 500,
        MaxInFlight = 5
    }).Build();

    /// <inheritdoc/>
    public async Task ProduceAsync(OperationDto operation, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(operation, _jsonSerializerOptions);
        var result = await _kafkaProducer.ProduceAsync(_kafkaOptions.TopicName, new Message<Null, string> { Value = json }, ct);
        logger.LogInformation("Produced {@Operation} with Value {Value} at Timestamp {Timestamp}. Status {Status}", operation, result.Value, result.Timestamp.UtcDateTime, result.Status);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _kafkaProducer.Dispose();
    }
}