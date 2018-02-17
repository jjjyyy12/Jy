using Jy.EntityFramewordCoreBase.Repositories;
using Jy.IRepositories;
using Microsoft.EntityFrameworkCore;
 
namespace Jy.EntityFrameworkCore.Repositories
{
    public class AuthRepositoryContext : EntityFrameworkRepositoryContext
    {
        public AuthRepositoryContext(JyDbContext session) : base(session)
        {
        }
    }
}
