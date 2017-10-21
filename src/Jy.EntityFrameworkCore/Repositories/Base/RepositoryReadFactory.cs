using Jy.Domain.IRepositories;
using Jy.EntityFramewordCoreBase.Connection;
using Jy.IRepositories;
using Microsoft.Extensions.Options;

namespace Jy.EntityFrameworkCore.Repositories
{
    public class RepositoryReadFactory : IRepositoryReadFactory
    {
        private readonly IOptionsSnapshot<SDBSettings> _SDBSettings;
        private readonly CreateRepository _createRepository = new CreateRepository();
        public RepositoryReadFactory(IOptionsSnapshot<SDBSettings> SDBSettings) 
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
        private JyDBReadContext getReadContext(string Id)
        {
            return BuildDBContext.CreateJyDBReadContextFromId(Id, _SDBSettings.Value.connectionKeyList, _SDBSettings.Value.connectionReadList, _SDBSettings.Value.defaultReadConnectionString, _SDBSettings.Value.dbType);
        }

        public TH CreateRepository<T, TH>(string Id)
            where T : Entity
            where TH : IRepositoryRead<T>
        {
            var contextObj = getReadContext(Id);
            return _createRepository.Get<TH>(new object[] { contextObj });
        }
        
    }
}
