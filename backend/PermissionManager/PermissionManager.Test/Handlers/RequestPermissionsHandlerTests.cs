using Moq;
using PermissionManager.Application.Commands;
using PermissionManager.Application.Commands.RequestPermission;
using PermissionManager.Application.DTOs;
using PermissionManager.Application.Interfaces;
using PermissionManager.Domain.Entities;
using PermissionManager.Domain.Interfaces;

namespace PermissionManager.Test.Handlers;

public class RequestPermissionHandlerTests
{
    [Fact]
    public async Task Handle_ShouldSaveIndexAndProduce_AndReturnNewId_WithinTransaction()
    {
        // Arrange
        var uowMock       = new Mock<IUnitOfWork>();
        var repoMock      = new Mock<IPermissionRepository>();
        var ftsMock       = new Mock<IFullTextSearchService>();
        var producerMock  = new Mock<IProducerService>();

        Permission? captured = null;
        repoMock
          .Setup(r => r.Add(It.IsAny<Permission>()))
          .Callback<Permission>(p => captured = p);

        uowMock.SetupGet(u => u.Permissions).Returns(repoMock.Object);

        uowMock
          .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
          .Callback<CancellationToken>(_ =>
          {
              if (captured != null)
                  captured.Id = 123;
          })
          .ReturnsAsync(1);

        uowMock
          .Setup(u => u.RunTransactionAsync(
              It.IsAny<Func<CancellationToken, Task<int>>>(),
              It.IsAny<CancellationToken>()))
          .Returns((Func<CancellationToken, Task<int>> func, CancellationToken ct) => func(ct));

        var cmd = new RequestPermissionCommand
        {
            EmployeeName     = "Jon",
            EmployeeSurname  = "Doe",
            PermissionTypeId = 5
        };

        var handler = new RequestPermissionHandler(
            uowMock.Object,
            ftsMock.Object,
            producerMock.Object
        );

        // Act
        var resultId = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.Equal(123, resultId);
        Assert.NotNull(captured);
        Assert.Equal("Jon", captured!.EmployeeName);
        Assert.Equal("Doe", captured.EmployeeSurname);
        Assert.Equal(5, captured.PermissionTypeId);

        repoMock.Verify(r => r.Add(It.IsAny<Permission>()), Times.Once);
        uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);

        ftsMock.Verify(f => f.IndexPermissionAsync(captured, It.IsAny<CancellationToken>()), Times.Once);
        producerMock.Verify(p => p.ProduceAsync(
            It.Is<OperationDto>(o => o.OperationName == OperationType.Request),
            It.IsAny<CancellationToken>()),
            Times.Once
        );
        
        ftsMock.Verify(f => f.DeletePermissionAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenProducerFails_ShouldDeleteFromFtsAndRollbackSql()
    {
        // Arrange
        var uowMock       = new Mock<IUnitOfWork>();
        var repoMock      = new Mock<IPermissionRepository>();
        var ftsMock       = new Mock<IFullTextSearchService>();
        var producerMock  = new Mock<IProducerService>();

        // Capture the permission entity
        Permission? captured = null;
        repoMock
            .Setup(r => r.Add(It.IsAny<Permission>()))
            .Callback<Permission>(p => captured = p);

        uowMock.SetupGet(u => u.Permissions).Returns(repoMock.Object);

        // Simulate CommitAsync assigning the ID
        uowMock
            .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Callback<CancellationToken>(_ =>
            {
                if (captured != null)
                    captured.Id = 456;
            })
            .ReturnsAsync(1);

        // Simulate FTS indexing succeeding
        ftsMock
            .Setup(f => f.IndexPermissionAsync(It.IsAny<Permission>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Simulate producer failure
        producerMock
            .Setup(p => p.ProduceAsync(It.IsAny<OperationDto>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Kafka error"));

        // Mock RunTransactionAsync to execute the delegate and propagate exception
        uowMock
            .Setup(u => u.RunTransactionAsync(
                It.IsAny<Func<CancellationToken, Task<int>>>(),
                It.IsAny<CancellationToken>()))
            .Returns(async (Func<CancellationToken, Task<int>> func, CancellationToken ct) => await func(ct));

        var cmd = new RequestPermissionCommand
        {
            EmployeeName     = "Error",
            EmployeeSurname  = "Case",
            PermissionTypeId = 7
        };

        var handler = new RequestPermissionHandler(
            uowMock.Object,
            ftsMock.Object,
            producerMock.Object
        );

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => handler.Handle(cmd, CancellationToken.None));
        Assert.Equal("Kafka error", ex.Message);

        // Verify the entity was captured and had an ID assigned
        Assert.NotNull(captured);
        Assert.Equal(456, captured!.Id);

        // Verify SQL insert and commit were attempted
        repoMock.Verify(r => r.Add(It.IsAny<Permission>()), Times.Once);
        uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);

        // Verify FTS indexing happened
        ftsMock.Verify(f => f.IndexPermissionAsync(captured!, It.IsAny<CancellationToken>()), Times.Once);

        // Verify FTS deletion as compensation
        ftsMock.Verify(f => f.DeletePermissionAsync(456, It.IsAny<CancellationToken>()), Times.Once);

        // Verify producer was attempted
        producerMock.Verify(p => p.ProduceAsync(
            It.Is<OperationDto>(o => o.OperationName == OperationType.Request),
            It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

}
