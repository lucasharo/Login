using Infra.Interfaces;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Dapper;
using Dapper.Contrib.Extensions;
using Entities.Entities;

namespace Infra.Repositories
{
    public class UsuarioRepository : Repository, IUsuarioRepository
    {
        public UsuarioRepository(SqlConnection sqlConnection, SqlTransaction transaction) : base(sqlConnection, transaction)
        {
        }

        public Usuario Inserir(Usuario usuario)
        {
            var result = _sqlConnection.Insert(usuario, _sqlTransaction);

            return usuario;
        }

        public bool Atualizar(Usuario usuario)
        {
            var result = _sqlConnection.Update(usuario, _sqlTransaction);

            return result;
        }

        public Usuario Get(int id)
        {
            string query = @"SELECT * FROM USUARIO WHERE ID = @ID";

            var result = _sqlConnection.QueryFirstOrDefault<Usuario>(query, new { ID = id }, transaction: _sqlTransaction);

            return result;
        }

        public Usuario GetByEmail(string email)
        {
            string query = @"SELECT * FROM USUARIO WHERE EMAIL = @EMAIL";

            var result = _sqlConnection.QueryFirstOrDefault<Usuario>(query, new { EMAIL = email }, transaction: _sqlTransaction);

            return result;
        }

        public Usuario GetByIdFacebook(long id)
        {
            string query = @"SELECT * FROM USUARIO WHERE IDFACEBOOK = @IDFACEBOOK";

            var result = _sqlConnection.QueryFirstOrDefault<Usuario>(query, new { IDFACEBOOK = id }, transaction: _sqlTransaction);

            return result;
        }

        public IEnumerable<Usuario> Listar()
        {
            string query = @"SELECT * FROM USUARIO";

            var result = _sqlConnection.Query<Usuario>(query, transaction: _sqlTransaction);

            return result;
        }

        public bool Deletar(Usuario usuario)
        {
            var result = _sqlConnection.Delete(usuario, _sqlTransaction);

            return result;
        }
    }
}