using Moq;
using PermissionManager.Application.Commands.ModifyPermission;
using PermissionManager.Application.DTOs;
using PermissionManager.Application.Interfaces;
using PermissionManager.Domain.Entities;
using PermissionManager.Domain.Interfaces;

namespace PermissionManager.Test.Handlers;

public class ModifyPermissionHandlerTests
{
    [Fact]
    public async Task Handle_ShouldUpdateIndexAndProduce_AndReturnSameId()
    {
        // Arrange
        var uowMock = new Mock<IUnitOfWork>();
        var repoMock = new Mock<IPermissionRepository>();
        var ftsMock = new Mock<IFullTextSearchService>();
        var producerMock = new Mock<IProducerService>();

        var existing = new Permission
        {
            Id = 99,
            EmployeeName = "Before",
            EmployeeSurname = "User",
            PermissionTypeId = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        repoMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        uowMock.SetupGet(u => u.Permissions).Returns(repoMock.Object);
        uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Make RunTransactionAsync invoke its internal delegate
        uowMock.Setup(u => u.RunTransactionAsync(It.IsAny<Func<CancellationToken, Task<int>>>(), It.IsAny<CancellationToken>()))
            .Returns((Func<CancellationToken, Task<int>> func, CancellationToken ct) => func(ct));

        var cmd = new ModifyPermissionCommand
        {
            Id = 99,
            EmployeeName = "After",
            EmployeeSurname = "User2",
            PermissionTypeId = 2
        };

        var handler = new ModifyPermissionHandler(
            uowMock.Object,
            ftsMock.Object,
            producerMock.Object
        );

        // Act
        var resultId = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.Equal(99, resultId);
        Assert.Equal("After", existing.EmployeeName);
        Assert.Equal("User2", existing.EmployeeSurname);
        Assert.Equal(2, existing.PermissionTypeId);

        repoMock.Verify(r => r.Update(existing), Times.Once);
        uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        ftsMock.Verify(f => f.IndexPermissionAsync(existing, It.IsAny<CancellationToken>()), Times.Once);
        ftsMock.Verify(f => f.DeletePermissionAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        producerMock.Verify(p => p.ProduceAsync(
            It.Is<OperationDto>(o => o.OperationName == OperationType.Modify),
            It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ShouldRollbackTransactionAndDeleteFromFts_WhenKafkaFails()
    {
        // Arrange
        var uowMock = new Mock<IUnitOfWork>();
        var repoMock = new Mock<IPermissionRepository>();
        var ftsMock = new Mock<IFullTextSearchService>();
        var producerMock = new Mock<IProducerService>();

        var existing = new Permission { Id = 99,  EmployeeName = "Before", EmployeeSurname = "User", PermissionTypeId = 1 };

        repoMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        uowMock.SetupGet(u => u.Permissions).Returns(repoMock.Object);
        uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Simulate Kafka throwing
        producerMock.Setup(p => p.ProduceAsync(It.IsAny<OperationDto>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Kafka failed"));

        // Make RunTransactionAsync execute the delegate
        uowMock.Setup(u => u.RunTransactionAsync(It.IsAny<Func<CancellationToken, Task<int>>>(), It.IsAny<CancellationToken>()))
            .Returns((Func<CancellationToken, Task<int>> func, CancellationToken ct) => func(ct));

        var handler = new ModifyPermissionHandler(uowMock.Object, ftsMock.Object, producerMock.Object);
        var cmd = new ModifyPermissionCommand { Id = 99, EmployeeName = "A" };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(cmd, CancellationToken.None));

        ftsMock.Verify(f => f.DeletePermissionAsync(existing.Id, It.IsAny<CancellationToken>()), Times.Once);
        uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task Handle_ShouldThrow_WhenPermissionNotFound()
    {
        // Arrange
        var uowMock = new Mock<IUnitOfWork>();
        var repoMock = new Mock<IPermissionRepository>();
        repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((Permission?)null);
        uowMock.SetupGet(u => u.Permissions).Returns(repoMock.Object);

        var handler = new ModifyPermissionHandler(
            uowMock.Object,
            Mock.Of<IFullTextSearchService>(),
            Mock.Of<IProducerService>()
        );
        var cmd = new ModifyPermissionCommand { Id = 42 };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(cmd, CancellationToken.None));
    }
}
