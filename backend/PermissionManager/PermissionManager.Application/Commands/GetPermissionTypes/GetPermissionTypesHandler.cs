using AutoMapper;
using MediatR;
using PermissionManager.Application.DTOs;
using PermissionManager.Domain.Interfaces;

namespace PermissionManager.Application.Commands.GetPermissionTypes;

/// <summary>
/// Handles the retrieval of all permission types.
/// <param name="unitOfWork">The unit of work for data access.</param>
/// <param name="mapper">The AutoMapper instance for mapping entities.</param>
/// </summary>
public class GetPermissionTypesHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetPermissionTypesQuery, IEnumerable<PermissionTypeDto>>
{
    /// <summary>
    /// Handles the GetPermissionTypesQuery request.
    /// <param name="request">The query for permission types.</param>
    /// <param name="cancellationToken">Token for cancelling the operation.</param>
    /// <returns>An enumerable of permission type DTOs.</returns>
    /// </summary>
    public async Task<IEnumerable<PermissionTypeDto>> Handle(GetPermissionTypesQuery request, CancellationToken cancellationToken)
    {
        var list = await unitOfWork.PermissionTypes.GetAllAsync(cancellationToken);
        return mapper.Map<List<PermissionTypeDto>>(list);
    }
}