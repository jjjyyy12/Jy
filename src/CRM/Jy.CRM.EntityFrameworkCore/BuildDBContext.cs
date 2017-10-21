
using Jy.EntityFramewordCoreBase.Connection;
using Jy.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text;

namespace Jy.CRM.EntityFrameworkCore
{
    public class BuildDBContext
    {
        public static JyCRMDBContext CreateJyDBContext(string connectionString, DBType dbType = DBType.MySql)
        {
            DbContextOptions<JyCRMDBContext> options = BuildConnection.CreateDbContextOptions<JyCRMDBContext>(connectionString, dbType);
            return new JyCRMDBContext(options);
        }
        public static JyCRMDBContext CreateJyDBContextFromId(string id, string connectionKeyList, string connectionList ,string defaultConnStr,DBType dbType = DBType.MySql)
        {
            var connectionString = string.IsNullOrWhiteSpace(id)? defaultConnStr : ConnectionHelper.getConnectionFromId(id,",",connectionKeyList, connectionList);
            DbContextOptions<JyCRMDBContext> options = BuildConnection.CreateDbContextOptions<JyCRMDBContext>(connectionString, dbType);
            return new JyCRMDBContext(options);
        }
        public static JyCRMDBReadContext CreateJyCRMDBReadContextFromId(string id, string connectionKeyList, string connectionList, string defaultConnStr, DBType dbType = DBType.MySql)
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
            DbContextOptions<JyCRMDBReadContext> options = BuildConnection.CreateDbContextOptions<JyCRMDBReadContext>(connectionString, dbType);
            return new JyCRMDBReadContext(options);
        }
        public static List<JyCRMDBContext> CreateAllJyDBContext(string connectionList, DBType dbType = DBType.MySql)
        {
            if (string.IsNullOrWhiteSpace(connectionList)) return null;
            string[] connectionString = connectionList.Split(',');
            HashSet<string> connSet = new HashSet<string>();
            List<JyCRMDBContext> rlist = new List<JyCRMDBContext>();
            for (int i = 0, j = connectionString.Length; i < j; i++)
            {
                var connStr = connectionString[i];
                if (connSet.Contains(connStr))
                    continue;
                connSet.Add(connStr);
                var option = BuildConnection.CreateDbContextOptions<JyCRMDBContext>(connStr, dbType);
                if (option != null)
                    rlist.Add(new JyCRMDBContext(option));
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
