
using Jy.DapperBase;
using Jy.DapperBase.Repositories;
 
 
namespace Jy.Dapper.Repositories
{
    public class AuthDPRepositoryReadContext : DapperRepositoryReadContext
    {
        public AuthDPRepositoryReadContext(TransactedConnection session) : base(session)
        {
        }
    }
}
