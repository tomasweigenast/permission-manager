using AutoMapper;
using MediatR;
using PermissionManager.Application.DTOs;
using PermissionManager.Domain.Interfaces;

namespace PermissionManager.Application.Commands.GetPermissionTypes;

public class GetPermissionTypesHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetPermissionTypesQuery, IEnumerable<PermissionTypeDto>>
{
    public async Task<IEnumerable<PermissionTypeDto>> Handle(GetPermissionTypesQuery request, CancellationToken cancellationToken)
    {
        var list = await unitOfWork.PermissionTypes.GetAllAsync(cancellationToken);
        return mapper.Map<List<PermissionTypeDto>>(list);
    }
}