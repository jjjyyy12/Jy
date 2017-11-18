using AutoMapper;
using Jy.Domain.Dtos;
using Jy.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.Domain
{
    /// <summary>
    /// Enity与Dto映射
    /// </summary>
    public class DomainMapper
    {
        public static void Initialize()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<AuthProfile>();
               
            });
        }
    }
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<Menu, MenuDto>();
            CreateMap<MenuDto, Menu>();

            CreateMap<Department, DepartmentDto>();
            CreateMap<DepartmentDto, Department>();

            CreateMap<User, UserDto>()
                .ForMember(dest => dest.CreateUserName, opt => opt.MapFrom(src => src.CreateUser.UserName))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EMail)) 
                .AfterMap((src , dest) => {
                    if (src.UserRoles != null)
                    {
                        List<Guid> roles = new List<Guid>();
                        foreach (var dto in src?.UserRoles)
                        {
                            roles.Add(dto.RoleId);
                        }
                        dest.RoleIds = roles;
                    }
                });

            CreateMap<UserDto, User>()
                .ForMember(dest => dest.EMail, opt => opt.MapFrom(src => src.Email));

            CreateMap<Role, RoleDto>();
            CreateMap<RoleDto, Role>();

            CreateMap<RoleMenuDto, RoleMenu>();
            CreateMap<RoleMenu, RoleMenuDto>()
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Menu.Url))
                .ForMember(dest => dest.MenuName, opt => opt.MapFrom(src => src.Menu.Name));

            CreateMap<UserRoleDto, UserRole>();
            CreateMap<UserRole, UserRoleDto>();

            CreateMap<RoleMenuDto, string>().ConvertUsing(source => source.Url ?? string.Empty);

            CreateMap<User, UserIndex>().ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));
            CreateMap<UserIndex, User>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId));

            CreateMap<User, UserIndexs>().ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));
            CreateMap<UserIndexs, User>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId));

            CreateMap<UserDto, UserIndex>().ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));
            CreateMap<UserIndex, UserDto>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId));
        }
    }
}
