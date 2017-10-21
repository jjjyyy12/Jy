using Jy.CRM.Domain.Entities;
using Jy.CRM.Domain.IRepositories;
using Jy.EntityFramewordCoreBase.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.CRM.EntityFrameworkCore.Repositories
{
    public class UserRepositoryRead : JyRepositoryReadBase<User, JyCRMDBReadContext>, IUserRepositoryRead
    {
        public UserRepositoryRead(JyCRMDBReadContext dbcontext) : base(dbcontext)
        {

        }

        public List<UserAddress> GetUserAddresss(Guid userId)
        {
            var query = from x in _dbContext.UserAddresss
                        where userId.Equals(x.UserId)
                        join y in _dbContext.Users on x.UserId equals y.Id into temp
                        from y in temp.DefaultIfEmpty()
                        join z in _dbContext.Addresss on x.AddressId equals z.Id into temp1
                        from z in temp1.DefaultIfEmpty()
                        select new UserAddress() { UserId = x.UserId, AddressId = x.AddressId, User = y, Address = z };
            var dd = query.Cast<UserAddress>().ToList();
            return dd;
        }
    }
}
