using Jy.Domain.Entities;
using Jy.Domain.IRepositories;
using Jy.EntityFramewordCoreBase.Repositories;
using Jy.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.EntityFrameworkCore.Repositories
{
    public class MenuRepositoryRead : EntityFrameworkRepositoryReadBase<Menu>, IMenuRepositoryRead
    {
        public MenuRepositoryRead(IRepositoryReadContext dbcontext) : base(dbcontext)
        {

        }
    }
}
