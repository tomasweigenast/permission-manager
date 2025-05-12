using PermissionManager.Application.DTOs;

namespace PermissionManager.Application.Interfaces;

/// <summary>
/// Defines a contract for producing or publishing <see cref="OperationDto"/> messages
/// to a message broker or streaming system (e.g., Kafka, RabbitMQ).
/// </summary>
public interface IProducerService
{
    /// <summary>
    /// Publishes the given <see cref="OperationDto"/> message to the configured message broker.
    /// </summary>
    /// <param name="operation">The operation payload to be serialized and sent.</param>
    /// <param name="ct">Optional <see cref="CancellationToken"/> to cancel the publish operation.</param>
    /// <returns>A task representing the asynchronous publish operation.</returns>
    Task ProduceAsync(OperationDto operation, CancellationToken ct = default);
}
