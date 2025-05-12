using AutoMapper;
using PermissionManager.Application.Commands;
using PermissionManager.Application.Commands.GetPermissions;
using PermissionManager.Application.Commands.ModifyPermission;
using PermissionManager.Application.Commands.RequestPermission;
using PermissionManager.Application.DTOs;
using PermissionManager.Core.Pagination;
using PermissionManager.Domain.Entities;

namespace PermissionManager.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<PermissionType, PermissionTypeDto>().ReverseMap();
        CreateMap<Permission, PermissionDto>()
            .ForMember(dest => dest.GrantedAt, a => a.MapFrom(p => p.CreatedAt))
            .ReverseMap();
        CreateMap<Permission, RequestPermissionDto>().ReverseMap();
        CreateMap<Permission, ModifyPermissionDto>().ReverseMap();

        CreateMap<RequestPermissionDto, RequestPermissionCommand>();
        CreateMap<ModifyPermissionDto, ModifyPermissionCommand>();
        
        CreateMap(typeof(PagedList<>), typeof(PagedList<>)).ConvertUsing(typeof(PagedListConverter<,>));
    }
}