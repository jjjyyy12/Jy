using Jy.EntityFramewordCoreBase.Repositories;
using Jy.IRepositories;
using Microsoft.EntityFrameworkCore;
 
namespace Jy.CRM.EntityFrameworkCore.Repositories
{
    public class CRMRepositoryContext : EntityFrameworkRepositoryContext
    {

        public CRMRepositoryContext(JyCRMDBContext session) : base(session)
        {
        }
    }
}
