using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using PermissionManager.Application.Commands.GetPermissions;
using PermissionManager.Application.DTOs;
using PermissionManager.Application.Interfaces;
using PermissionManager.Core.Pagination;
using PermissionManager.Domain.Entities;
using PermissionManager.Domain.Interfaces;

namespace PermissionManager.Test.Handlers;

public class GetPermissionsHandlerTests
{
    [Fact]
    public async Task Handle_ShouldFetchProduceAndReturnDtoList()
    {
        // Arrange
        var uowMock = new Mock<IUnitOfWork>();
        var repoMock = new Mock<IPermissionRepository>();
        var producerMock = new Mock<IProducerService>();
        var mapperMock = new Mock<IMapper>();
        var loggerMock = new Mock<ILogger<GetPermissionsHandler>>();

        // Sample data
        var listEntity = new PagedList<Permission>(
            [
                new Permission { Id = 1, EmployeeName = "Jon", EmployeeSurname = "Doe", PermissionTypeId = 1, PermissionType = new PermissionType {Id = 1, Description = "Permission 1"}},
                new Permission { Id = 2, EmployeeName = "Jane", EmployeeSurname = "Doe", PermissionTypeId = 2, PermissionType = new PermissionType {Id = 2, Description = "Permission 2"} }
            ]);
        repoMock.Setup(r => r.GetAllAsync(100,1,null, It.IsAny<CancellationToken>())).ReturnsAsync(listEntity);
        uowMock.SetupGet(u => u.Permissions).Returns(repoMock.Object);

        // Map each Permission -> PermissionDto
        var listDto = new PagedList<PermissionDto>(
            [
                new PermissionDto { Id = 1, EmployeeName = "Jon", EmployeeSurname = "Doe", PermissionType = new PermissionTypeDto{Id = 1, Description = "Permission 1"} },
                new PermissionDto { Id = 2, EmployeeName = "Jane", EmployeeSurname = "Doe", PermissionType  = new  PermissionTypeDto{ Id = 2, Description = "Permission 2"} }
            ]);
        mapperMock.Setup(m => m.Map<PagedList<PermissionDto>>(listEntity)).Returns(listDto);

        var handler = new GetPermissionsHandler(
            uowMock.Object,
            mapperMock.Object,
            producerMock.Object,
            loggerMock.Object
        );

        var query = new GetPermissionsQuery { Page = 1, PageSize = 100 };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(listDto, result);
        repoMock.Verify(r => r.GetAllAsync(100, 1, null, It.IsAny<CancellationToken>()), Times.Once);
        producerMock.Verify(p => p.ProduceAsync(
            It.Is<OperationDto>(o => o.OperationName == OperationType.Get),
            It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
