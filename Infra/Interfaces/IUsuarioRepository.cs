using Entities.Entities;
using System.Collections.Generic;

namespace Infra.Interfaces
{
    public interface IUsuarioRepository
    {
        Usuario Inserir(Usuario usuario);
        bool Atualizar(Usuario usuario);
        Usuario Get(int id);
        Usuario GetByEmail(string email);
        Usuario GetByIdFacebook(long id);
        IEnumerable<Usuario> Listar();
        bool Deletar(Usuario usuario);
    }
}