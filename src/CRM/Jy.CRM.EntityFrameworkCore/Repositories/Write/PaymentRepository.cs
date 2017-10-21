using Jy.CRM.Domain.Entities;
using Jy.CRM.Domain.IRepositories;
using Jy.EntityFramewordCoreBase.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.CRM.EntityFrameworkCore.Repositories
{
    public class PaymentRepository : JyRepositoryBase<Payment, JyCRMDBContext>, IPaymentRepository
    {
        public PaymentRepository(JyCRMDBContext dbcontext) : base(dbcontext, dbcontext)
        {

        }
    }
}
