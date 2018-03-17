using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Data;
using Jy.IRepositories;

namespace Jy.DapperBase.Connection
{
    public class BuildConnection
    {
        public static IDbConnection GetConnection(string connectionString, DBType dbType = DBType.MySql,bool open = true,
            bool convertZeroDatetime = false, bool allowZeroDatetime = false)
        {
            IDbConnection connection;
            switch (dbType)
            {
                case DBType.MySql:
                    var csb = new MySqlConnectionStringBuilder(connectionString)
                    {
                        AllowZeroDateTime = allowZeroDatetime,
                        ConvertZeroDateTime = convertZeroDatetime
                    };
                    connection = new MySqlConnection(csb.ConnectionString);
                    break;
                case DBType.SqlServer:
                    connection = new SqlConnection(connectionString);
                    break;
                default:
                    connection = new MySqlConnection(connectionString);
                    break;
            }
            if (open) connection.Open();
            return connection;
        }
    }
}
