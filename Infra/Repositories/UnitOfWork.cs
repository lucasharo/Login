using Infra.Interfaces;
using Microsoft.Data.SqlClient;
using System;

namespace Infra.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private SqlConnection _sqlConnection;
        private SqlTransaction _sqlTransaction;
        public IUsuarioRepository UsuarioRepository { get; }

        public UnitOfWork(SqlConnection sqlConnection, SqlTransaction sqlTransaction)
        {
            _sqlConnection = sqlConnection;
            _sqlTransaction = sqlTransaction;

            UsuarioRepository = new UsuarioRepository(sqlConnection, sqlTransaction);
        }

        public void Commit()
        {
            _sqlTransaction.Commit();
        }
        public void Dispose()
        {
            _sqlConnection.Dispose();
        }
    }
}