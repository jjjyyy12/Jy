using Jy.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Jy.EntityFramewordCoreBase.Connection
{
    public class BuildConnection
    {
        public static DbContextOptions<T> CreateDbContextOptions<T>(string connectionString, DBType dbType = DBType.MySql) where T:DbContext
        {
            var builder = new DbContextOptionsBuilder<T>();
            switch (dbType)
            {
                case DBType.MySql:
                    return builder.UseMySql(connectionString).Options;

                default:
                    return builder.UseMySql(connectionString).Options;
            }
        }
        
    }
}
