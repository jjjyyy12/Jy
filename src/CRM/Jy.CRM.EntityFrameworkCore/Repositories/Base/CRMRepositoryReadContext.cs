using Jy.EntityFramewordCoreBase.Repositories;
using Jy.IRepositories;
using Microsoft.EntityFrameworkCore;
 
namespace Jy.CRM.EntityFrameworkCore.Repositories
{
    public class CRMRepositoryReadContext : EntityFrameworkRepositoryReadContext
    {

        public CRMRepositoryReadContext(JyCRMDBReadContext session) : base(session)
        {
        }
    }
}
