using Jy.CRM.Domain.Entities;
using Jy.CRM.Domain.IRepositories;
using Jy.EntityFramewordCoreBase.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.CRM.EntityFrameworkCore.Repositories
{
    public class AddressRepositoryRead : JyRepositoryReadBase <Address,int, JyCRMDBReadContext>, IAddressRepositoryRead
    {
        public AddressRepositoryRead(JyCRMDBReadContext dbcontext) : base(dbcontext)
        {

        }

    }
}
