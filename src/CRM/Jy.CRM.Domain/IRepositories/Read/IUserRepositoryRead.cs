using Jy.CRM.Domain.Entities;
using Jy.IRepositories;
using System;
using System.Collections.Generic;

namespace Jy.CRM.Domain.IRepositories
{
    public interface IUserRepositoryRead : IRepositoryRead<User>
    {
        List<UserAddress> GetUserAddresss(Guid userId);
    }
}
