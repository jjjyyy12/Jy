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
    public class DepartmentRepository : EntityFrameworkRepositoryBase<Department>, IDepartmentRepository
    {
        public DepartmentRepository(IRepositoryContext dbcontext) : base(dbcontext)
        {

        }
    }
}
