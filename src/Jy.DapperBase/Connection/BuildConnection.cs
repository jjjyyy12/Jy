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
        public static IDbConnection GetConnection(string connectionString, DBType dbType = DBType.MySql)
        {
            IDbConnection connection;
            switch (dbType)
            {
                case DBType.MySql:
                    connection = new MySqlConnection(connectionString);
                    break;
                case DBType.SqlServer:
                    connection = new SqlConnection(connectionString);
                    break;
                default:
                    connection = new MySqlConnection(connectionString);
                    break;
            }
            connection.Open();
            return connection;
        }
    }
}
