using Jy.IIndex;
using Jy.SolrIndex.Connection;
using SolrNetCore.Impl;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jy.AuthAdmin.SolrIndex
{
    public class BuildSolrServer<T>  
    {
        public static AuthAdminSolrServer<T> CreateSolrServer(string connectionString,string coreIndex, IndexType indexType = IndexType.Solr)
        {
            var connection = BuildConnection.CreateSolrConnection(connectionString + coreIndex, indexType);
            var basesolr = new SolrBasicServer<T>(connection, null, null, null, null, null, null, null);
            return new AuthAdminSolrServer<T>(basesolr,null,null);
        }
        //根据ID得到分库的context
        public static AuthAdminSolrServer<T> CreateSolrServerFromId(string id, string coreIndex, string connectionKeyList, string connectionList, string defaultConnStr, IndexType indexType = IndexType.Solr)
        {
            var connectionString = string.IsNullOrWhiteSpace(id) ? defaultConnStr : ConnectionHelper.getConnectionFromId(id, ",", connectionKeyList, connectionList);
            return CreateSolrServer(connectionString, coreIndex, indexType);
        }
        //根据ID得到分库的slave的context，如果一个分库有2个slave，根据idhash出读哪个库
        public static AuthAdminSolrServer<T> CreateSolrReadServerFromId(string id, string coreIndex, string connectionKeyList, string connectionList, string defaultConnStr, IndexType indexType = IndexType.Solr)
        {
            var connectionString = string.IsNullOrWhiteSpace(id) ? defaultConnStr : ConnectionHelper.getConnectionFromId(id, ",", connectionKeyList, connectionList);
            if (connectionString.IndexOf("^") > 0)
            {
                int slavecount = SubstringCount(connectionString, "^");
                StringBuilder slaveKeyList = new StringBuilder();
                for (int i = 1; i <= slavecount; i++)
                    slaveKeyList.Append(i.ToString()).Append("^");
                slaveKeyList.Remove(slaveKeyList.Length - 1, 1);
                connectionString = ConnectionHelper.getConnectionFromId(id, "^", slaveKeyList.ToString(), connectionString);
            }
            return CreateSolrServer(connectionString, coreIndex, indexType);
        }
        //获取所有分库context,除了主库
        public static List<AuthAdminSolrServer<T>> CreateAllSolrServer(string connectionList, string coreIndex, string defaultConnStr, IndexType indexType = IndexType.Solr)
        {
            if (string.IsNullOrWhiteSpace(connectionList)) return null;
            string[] connectionString = connectionList.Split(',');
            HashSet<string> connSet = new HashSet<string>();
            List<AuthAdminSolrServer<T>> rlist = new List<AuthAdminSolrServer<T>>();
            for (int i = 0, j = connectionString.Length; i < j; i++)
            {
                var connStr = connectionString[i];
                if (connSet.Contains(connStr) || defaultConnStr.Equals(connStr))
                    continue;
                connSet.Add(connStr);
                rlist.Add(CreateSolrServer(connStr, coreIndex, indexType));
            }
            return rlist;
        }
        public static HashSet<string> GetConnectionStrings(string connectionList, string defaultConnStr, IndexType indexType = IndexType.Solr)
        {
            if (string.IsNullOrWhiteSpace(connectionList)) return null;
            string[] connectionString = connectionList.Split(',');
            HashSet<string> connSet = new HashSet<string>();
            for (int i = 0, j = connectionString.Length; i < j; i++)
            {
                var connStr = connectionString[i];
                if (connSet.Contains(connStr) || defaultConnStr.Equals(connStr))
                    continue;
                connSet.Add(connStr);
            }
            return connSet;
        }
        private static int SubstringCount(string str, string substring)
        {
            if (str.Contains(substring))
            {
                string strReplaced = str.Replace(substring, "");
                return (str.Length - strReplaced.Length) / substring.Length;
            }
            return 0;
        }
    }
}
