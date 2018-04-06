
using Jy.DapperBase;
using Jy.DapperBase.Connection;
using Jy.DapperBase.Repositories;
using Jy.IRepositories;
using System.Collections.Generic;
using System.Text;

namespace Jy.Dapper
{
    public class BuildDBContext
    {
        public static TransactedConnection CreateJyDBContext(string connectionString, DBType dbType = DBType.MySql)
        {
            var conn = BuildConnection.GetConnection(connectionString, dbType);
            return new TransactedConnection(conn, conn.BeginTransaction());
        }
        //根据ID得到分库的context
        public static TransactedConnection CreateJyDBContextFromId(string id, string connectionKeyList, string connectionList ,string defaultConnStr,DBType dbType = DBType.MySql)
        {
            var connectionString = string.IsNullOrWhiteSpace(id)? defaultConnStr : ConnectionHelper.getConnectionFromId(id,",",connectionKeyList, connectionList);
            return CreateJyDBContext(connectionString, dbType);
        }
        //根据ID得到分库的slave的context，如果一个分库有2个slave，根据idhash出读哪个库
        public static TransactedConnection CreateJyDBReadContextFromId(string id, string connectionKeyList, string connectionList, string defaultConnStr, DBType dbType = DBType.MySql)
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
            return CreateJyDBContext(connectionString, dbType);
        }
        //获取所有分库context,除了主库
        public static List<TransactedConnection> CreateAllJyDBContext(string connectionList, string defaultConnStr,DBType dbType = DBType.MySql)
        {
            if (string.IsNullOrWhiteSpace(connectionList)) return null;
            string [] connectionString = connectionList.Split(',');
            HashSet<string> connSet = new HashSet<string>();
            List<TransactedConnection> rlist =new List<TransactedConnection>();
            for(int i = 0 ,j=connectionString.Length;i<j;i++)
            {
                var connStr = connectionString[i];
                if (connSet.Contains(connStr) || defaultConnStr.Equals(connStr))
                    continue;
                connSet.Add(connStr);
                if(!string.IsNullOrWhiteSpace(connStr) )
                    rlist.Add( CreateJyDBContext(connStr, dbType));
            }
            return rlist;
        }
        public static HashSet<string> GetConnectionStrings(string connectionList, string defaultConnStr, DBType dbType = DBType.MySql)
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
