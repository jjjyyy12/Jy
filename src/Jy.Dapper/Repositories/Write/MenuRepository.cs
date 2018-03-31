using Jy.DapperBase.Repositories;
using Jy.Domain.Entities;
using Jy.Domain.IRepositories; 
using Jy.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.Dapper.Repositories
{
    public class MenuRepository : DapperRepositoryBase<Menu>, IMenuRepository
    {
        public MenuRepository(IRepositoryContext dbcontext) : base(dbcontext)
        {

        }
    }
}
