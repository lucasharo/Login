using Microsoft.Data.SqlClient;

namespace Infra.Repositories
{
    public class Repository
    {
        protected SqlConnection _sqlConnection;
        protected SqlTransaction _sqlTransaction;

        public Repository(SqlConnection sqlConnection, SqlTransaction sqlTransaction)
        {
            _sqlConnection = sqlConnection;

            _sqlTransaction = sqlTransaction;
        }

        public Repository(SqlConnection sqlConnection)
        {
            _sqlConnection = sqlConnection;
        }

        protected SqlCommand CreateCommand()
        {
            var cmd = _sqlConnection.CreateCommand();

            cmd.Transaction = _sqlTransaction;

            return cmd;
        }
    }
}