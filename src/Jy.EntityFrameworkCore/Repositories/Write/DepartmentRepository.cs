using Jy.Domain.Entities;
using Jy.Domain.IRepositories;
using Jy.EntityFramewordCoreBase.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.EntityFrameworkCore.Repositories
{
    public class DepartmentRepository : JyRepositoryBase<Department, JyDbContext>, IDepartmentRepository
    {
        public DepartmentRepository(JyDbContext dbcontext) : base(dbcontext, dbcontext)
        {

        }
    }
}
