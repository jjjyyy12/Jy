using System;
using System.Collections.Generic;
using System.Text;

namespace Jy.DapperBase.Connection
{
    class BuildConnection
    {
        //public static string GetConnectionString(string strConnectionKey)
        //{
        //    string strConnectionString = string.Empty;
        //    if (ConfigurationManager.ConnectionStrings[strConnectionKey] != null)
        //    {
        //        strConnectionString = ConfigurationManager.ConnectionStrings[strConnectionKey].ConnectionString ?? string.Empty;
        //    }
        //    return strConnectionString;
        //}
        //public static IDbConnection GetConnection(string dbType, string cinemaID)
        //{
        //    IDbConnection connection;
        //    switch (dbType)
        //    {
        //        case "ORACLE":
        //            connection = new OracleConnection(GetConnectionString("dp_" + cinemaID));
        //            break;
        //        default:
        //            connection = new OracleConnection(GetConnectionString("dp_" + cinemaID));
        //            break;
        //    }
        //    connection.Open();
        //    return connection;
        //}
        //public static IDbConnection GetConnection(string dbType)
        //{
        //    return GetConnection(dbType, "Entities");
        //}
    }
}
