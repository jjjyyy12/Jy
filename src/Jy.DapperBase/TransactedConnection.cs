using Jy.IRepositories;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Jy.DapperBase
{
    public class TransactedConnection : IDbConnection, IUnitOfWork
    {
        private readonly IDbConnection _conn;
        private readonly IDbTransaction _tran;

        public IDbConnection connection { get { return _conn; } }

        public TransactedConnection(IDbConnection conn, IDbTransaction tran)
        {
            _conn = conn;
            _tran = tran;
        }

        public string ConnectionString
        {
            get { return _conn.ConnectionString; }
            set { _conn.ConnectionString = value; }
        }

        public int ConnectionTimeout => _conn.ConnectionTimeout;
        public string Database => _conn.Database;
        public ConnectionState State => _conn.State;

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            throw new NotImplementedException();
        }

        public IDbTransaction BeginTransaction() => _tran;

        public void ChangeDatabase(string databaseName) => _conn.ChangeDatabase(databaseName);

        public void Close() => _conn.Close();

        public IDbCommand CreateCommand()
        {
            // The command inherits the "current" transaction.
            var command = _conn.CreateCommand();
            command.Transaction = _tran;
            return command;
        }

        public void Dispose() => _conn.Dispose();

        public void Open() => _conn.Open();

        public int SaveChange()
        {
            _tran.Commit();
            return 1;
        }

        public Task SaveChangeAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Factory.StartNew(SaveChange, cancellationToken);
        }

    }
}
