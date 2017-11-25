using AutoMapper;
using Jy.Domain.Dtos;
using Jy.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.TokenService
{
    /// <summary>
    /// Enity与Dto映射
    /// </summary>
    public class JyMapper
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
            CreateMap<UserDto, User>()
            .ForMember(dest => dest.EMail, opt => opt.MapFrom(src => src.Email));

            CreateMap<User, UserDto>()
            .ForMember(dest => dest.CreateUserName, opt => opt.MapFrom(src => src.CreateUser.UserName))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EMail))
            .AfterMap((src, dest) => {
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

            CreateMap<RoleMenuDto, RoleMenu>();
            CreateMap<RoleMenu, RoleMenuDto>()
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Menu.Url))
                .ForMember(dest => dest.MenuName, opt => opt.MapFrom(src => src.Menu.Name));

            CreateMap<RoleMenuDto, string>().ConvertUsing(source => source.Url ?? string.Empty);

            CreateMap<User, UserIndex>().ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));
            CreateMap<UserIndex, User>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId));

            CreateMap<User, UserIndexs>().ForMember(dest => dest.name, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.keywords, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.depid, opt => opt.MapFrom(src => src.DepartmentId));
            CreateMap<UserIndexs, User>().ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.name))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.keywords))
                .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.depid));

            CreateMap<UserIndex, UserIndexs>().ForMember(dest => dest.name, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.keywords, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.depid, opt => opt.MapFrom(src => src.DepartmentId))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId));
            CreateMap<UserIndexs, UserIndex>().ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.name))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.keywords))
                .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.depid))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));
        }
    }
}
