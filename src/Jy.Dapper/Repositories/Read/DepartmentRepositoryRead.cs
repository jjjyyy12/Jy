using Jy.DapperBase.Repositories;
using Jy.Domain.Entities;
using Jy.Domain.IRepositories;
using Jy.IRepositories;

namespace Jy.Dapper.Repositories
{
    public class DepartmentRepositoryRead : DapperRepositoryReadBase<Department>, IDepartmentRepositoryRead
    {
        public DepartmentRepositoryRead(IRepositoryReadContext dbcontext) : base(dbcontext)
        {

        }
    }
}
