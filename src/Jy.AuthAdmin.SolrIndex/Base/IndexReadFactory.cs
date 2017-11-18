using Jy.Domain.IIndex;
using Jy.IIndex;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace Jy.AuthAdmin.SolrIndex
{
    public class IndexReadFactory<T> : IIndexReadFactory where T : Entity
    {
        private readonly IOptionsSnapshot<SIndexSettings> _SIndexSettings;
        private readonly CreateIndex _createIndex = new CreateIndex();
        public IndexReadFactory(IOptionsSnapshot<SIndexSettings> SIndexSettings)
        {
            _SIndexSettings = SIndexSettings;
            if (_createIndex?.ConfigDictionary.Count == 0)
            {
                _createIndex.AddConfig<IUserIndexsIndexRead, UserIndexsIndexRead>();
            }
        }
        private AuthAdminSolrServer<T> getSolrServer(string Id, string coreIndex)
        {
            return BuildSolrServer<T>.CreateSolrReadServerFromId(Id, coreIndex, _SIndexSettings.Value.connectionKeyList, _SIndexSettings.Value.connectionList,_SIndexSettings.Value.defaultConnectionString,_SIndexSettings.Value.indexType);
        }

        public TH CreateIndex<T, TH>(string Id, string coreIndex)
            where T : Entity
            where TH : IIndexRead<T>
        {
            var contextObj = getSolrServer(Id, coreIndex);
            return _createIndex.Get<TH>(new object[] { contextObj });
        }
      

       
    }
}
