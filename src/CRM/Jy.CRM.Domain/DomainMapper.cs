using AutoMapper;
using Jy.CRM.Domain.Dtos;
using Jy.CRM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.CRM.Domain
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
            CreateMap<SecKillOrder, SecKillOrderDto>();
            CreateMap<SecKillOrderDto, SecKillOrder>();

            CreateMap<UserAddress, UserAddressDto>();
            CreateMap<UserAddressDto, UserAddress>();

            CreateMap<Commodity, CommodityDto>();
            CreateMap<CommodityDto, Commodity>();

            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();

            CreateMap<Payment, PaymentDto>();
            CreateMap<PaymentDto, Payment>();
            
        }
    }
}
