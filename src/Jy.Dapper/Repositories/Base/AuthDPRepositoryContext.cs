
using Jy.DapperBase;
using Jy.DapperBase.Repositories;
 
 
namespace Jy.Dapper.Repositories
{
    public class AuthDPRepositoryContext : DapperRepositoryContext
    {
        public AuthDPRepositoryContext(TransactedConnection session) : base(session)
        {
        }
    }
}
