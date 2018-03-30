
using Dapper;
using System.Collections.Generic;
using System.Data;

namespace Jy.DapperBase.Repositories.Extensions
{
    public static partial class CRUDExtension
    {
        private static Dialect dialect = Dialect.MSSQL;

        public static IEnumerable<TEntity> List<TEntity>(
            this IDbConnection connection,
            object conditions = null,
            IDbTransaction transaction = null,
            bool buffered = true,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            var command = StatementFactory.Select<TEntity>(dialect, conditions);

            return connection.Query<TEntity>(command, conditions, transaction, buffered, commandTimeout, commandType);
        }
    }
}
