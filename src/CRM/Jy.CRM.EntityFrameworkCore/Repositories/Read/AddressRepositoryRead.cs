using Jy.CRM.Domain.Entities;
using Jy.CRM.Domain.IRepositories;
using Jy.EntityFramewordCoreBase.Repositories;
using Jy.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.CRM.EntityFrameworkCore.Repositories
{
    public class AddressRepositoryRead : EntityFrameworkRepositoryReadBase<Address,int>, IAddressRepositoryRead
    {
        public AddressRepositoryRead(IRepositoryReadContext dbcontext) : base(dbcontext)
        {

        }

    }
}
