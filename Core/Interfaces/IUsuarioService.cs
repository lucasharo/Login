using Entities.Entities;
using System.Collections.Generic;

namespace Core.Interfaces
{
    public interface IUsuarioService
    {
        UsuarioLogin CadastrarUsuario(Usuario usuario);
        UsuarioLogin AlterarUsuario(int id, Usuario usuario);
        IEnumerable<UsuarioLogin> Listar();
        UsuarioLogin GetById(int id);
    }
}
