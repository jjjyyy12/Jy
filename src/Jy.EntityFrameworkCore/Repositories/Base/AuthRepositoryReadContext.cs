using Jy.EntityFramewordCoreBase.Repositories;
using Jy.IRepositories;
using Microsoft.EntityFrameworkCore;
 
namespace Jy.EntityFrameworkCore.Repositories
{
    public class AuthRepositoryReadContext : EntityFrameworkRepositoryReadContext
    {
        public AuthRepositoryReadContext(JyDBReadContext session) : base(session)
        {
        }
    }
}
