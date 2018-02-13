using Jy.CRM.Domain.Entities;
using Jy.CRM.Domain.IRepositories;
using Jy.EntityFramewordCoreBase.Repositories;
using Jy.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.CRM.EntityFrameworkCore.Repositories
{
    public class PaymentRepository : EntityFrameworkRepositoryBase<Payment>, IPaymentRepository
    {
        public PaymentRepository(IRepositoryContext dbcontext) : base(dbcontext)
        {

        }
    }
}
