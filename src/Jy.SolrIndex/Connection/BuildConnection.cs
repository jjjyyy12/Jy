using Jy.IIndex;
using SolrNetCore.Impl;

namespace Jy.SolrIndex.Connection
{
    public class BuildConnection
    {
        public static SolrConnection CreateSolrConnection(string connectionString, IndexType indexType = IndexType.Solr)  
        {
            switch (indexType)
            {
                case IndexType.Solr:
                    return  new SolrConnection(connectionString)
                    {
                        //Cache = Container.GetInstance<ISolrCache>(),
                    };

                default:
                    return new SolrConnection(connectionString)
                    {
                        //Cache = Container.GetInstance<ISolrCache>(),
                    };
            }
        }
        
    }
}
