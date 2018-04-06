using Jy.DapperBase;
using Jy.DapperBase.Repositories;
using Jy.Domain.IRepositories;
using Jy.IRepositories;
using Microsoft.Extensions.Options;

namespace Jy.Dapper.Repositories
{
    public class DPRepositoryReadFactory : IRepositoryReadFactory
    {
        private readonly IOptionsSnapshot<SDBSettings> _SDBSettings;
        private readonly CreateRepository _createRepository = new CreateRepository();
        public DPRepositoryReadFactory(IOptionsSnapshot<SDBSettings> SDBSettings) 
        {
            _SDBSettings = SDBSettings;
            if (_createRepository?.ConfigDictionary.Count == 0)
            {
                _createRepository.AddConfig<IUserRepositoryRead, UserRepositoryRead>();
                _createRepository.AddConfig<IRoleRepositoryRead, RoleRepositoryRead>();
                _createRepository.AddConfig<IMenuRepositoryRead, MenuRepositoryRead>();
                _createRepository.AddConfig<IDepartmentRepositoryRead, DepartmentRepositoryRead>();
            }
        }
        private TransactedConnection getReadContext(string Id)
        {
            return BuildDBContext.CreateJyDBReadContextFromId(Id, _SDBSettings.Value.connectionKeyList, _SDBSettings.Value.connectionList, _SDBSettings.Value.defaultConnectionString, _SDBSettings.Value.dbType);
        }
        private DapperRepositoryReadContext getRepositoryReadContext(TransactedConnection context)
        {
            return new DapperRepositoryReadContext(context);
        }
        public TH CreateRepository<T, TH>(string Id)
            where T : Entity
            where TH : IRepositoryRead<T>
        {
            var contextObj = getReadContext(Id);
            var repositoryContext = getRepositoryReadContext(contextObj);
            return _createRepository.Get<TH>(new object[] { repositoryContext });
        }
        
    }
}
