using Jy.DapperBase.Repositories;
using Jy.Domain.Entities;
using Jy.Domain.IRepositories;
using Jy.IRepositories;

namespace Jy.Dapper.Repositories
{
    public class MenuRepositoryRead : DapperRepositoryReadBase<Menu>, IMenuRepositoryRead
    {
        public MenuRepositoryRead(IRepositoryReadContext dbcontext) : base(dbcontext)
        {

        }
    }
}
