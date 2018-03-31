using Jy.DapperBase.Repositories;
using Jy.Domain.Entities;
using Jy.Domain.IRepositories; 
using Jy.IRepositories;

namespace Jy.Dapper.Repositories
{
    public class DepartmentRepository : DapperRepositoryBase<Department>, IDepartmentRepository
    {
        public DepartmentRepository(IRepositoryContext dbcontext) : base(dbcontext)
        {

        }
    }
}
