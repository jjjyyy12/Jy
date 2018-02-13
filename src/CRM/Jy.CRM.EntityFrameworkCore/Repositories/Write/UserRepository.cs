﻿using Jy.CRM.Domain.Entities;
using Jy.CRM.Domain.IRepositories;
using Jy.EntityFramewordCoreBase.Repositories;
using Jy.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.CRM.EntityFrameworkCore.Repositories
{
    public class UserRepository : EntityFrameworkRepositoryBase<User>, IUserRepository
    {
        public UserRepository(IRepositoryContext dbcontext) : base(dbcontext)
        {

        }
    }
}
