using Jy.CRM.Domain.IRepositories;
using Jy.CRM.EntityFrameworkCore;
using Jy.CRM.EntityFrameworkCore.Repositories;
using Jy.EntityFramewordCoreBase.Repositories;
using Jy.IRepositories;
using Microsoft.Extensions.Options;

namespace Jy.CRM.EntityFrameworkCore.Repositories
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
                _createRepository.AddConfig<IAddressRepository, AddressRepository>();
                _createRepository.AddConfig<ICommodityRepository, CommodityRepository>();
                _createRepository.AddConfig<IPaymentRepository, PaymentRepository>();
                _createRepository.AddConfig<ISecKillOrderRepository, SecKillOrderRepository>();
            }
        }
        private JyCRMDBReadContext getReadContext(string Id)
        {
            return BuildDBContext.CreateJyCRMDBReadContextFromId(Id, _SDBSettings.Value.connectionKeyList, _SDBSettings.Value.connectionReadList, _SDBSettings.Value.defaultReadConnectionString, _SDBSettings.Value.dbType);
        }
        private EntityFrameworkRepositoryReadContext getRepositoryReadContext(JyCRMDBReadContext context)
        {
            return new EntityFrameworkRepositoryReadContext(context);
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
