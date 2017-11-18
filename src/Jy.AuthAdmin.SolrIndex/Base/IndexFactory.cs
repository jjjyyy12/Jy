using Jy.Domain.IIndex;
using Jy.IIndex;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace Jy.AuthAdmin.SolrIndex
{
    public class IndexFactory<T> : IIndexFactory where T : Entity
    {
        private readonly IOptionsSnapshot<SIndexSettings> _SIndexSettings;
        private readonly CreateIndex _createIndex = new CreateIndex();
        public IndexFactory(IOptionsSnapshot<SIndexSettings> SIndexSettings)
        {
            _SIndexSettings = SIndexSettings;
            if (_createIndex?.ConfigDictionary.Count == 0)
            {
                _createIndex.AddConfig<IUserIndexsIndex, UserIndexsIndex>();
            }
        }
        private AuthAdminSolrServer<T> getSolrServer(string Id, string coreIndex)
        {
            return BuildSolrServer<T>.CreateSolrServerFromId(Id, coreIndex, _SIndexSettings.Value.connectionKeyList, _SIndexSettings.Value.connectionList,_SIndexSettings.Value.defaultConnectionString,_SIndexSettings.Value.indexType);
        }
        public HashSet<string> GetConnectionStrings()
        {
           return BuildSolrServer<T>.GetConnectionStrings(_SIndexSettings.Value.connectionList,_SIndexSettings.Value.defaultConnectionString, _SIndexSettings.Value.indexType);
        }

        public TH CreateIndex<T, TH>(string Id, string coreIndex)
            where T : Entity
            where TH : IIndex<T>
        {
            var contextObj = getSolrServer(Id,coreIndex);
            return _createIndex.Get<TH>(new object[] { contextObj });
        }
        public TH CreateDefaultIndex<T, TH>(string coreIndex)
           where TH : IIndex<T>
           where T : Entity
        {
            var contextObj = BuildSolrServer<T>.CreateSolrServer(_SIndexSettings.Value.defaultConnectionString, coreIndex, _SIndexSettings.Value.indexType);
            return _createIndex.Get<TH>(new object[] { contextObj });
        }

        //得到所有分库的Index
        public List<TH> CreateAllIndex<T, TH>( string coreIndex)
            where T : Entity
            where TH : IIndex<T>
        {
            var contextList = BuildSolrServer<T>.CreateAllSolrServer(_SIndexSettings.Value.connectionList, coreIndex, _SIndexSettings.Value.defaultConnectionString,_SIndexSettings.Value.indexType);
            List<TH> rlist = new List<TH>();
            contextList.ForEach((contextObj) => { rlist.Add( _createIndex.Get<TH>(new object[] { contextObj })); });
            return rlist;
        }
        public TH CreateIndexByConnStr<T, TH>(string ConnStr, string coreIndex)
            where T : Entity
            where TH : IIndex<T>
        {
            var contextObj = BuildSolrServer<T>.CreateSolrServer(ConnStr, coreIndex, _SIndexSettings.Value.indexType);
            return _createIndex.Get<TH>(new object[] { contextObj });
        }
    }
}
