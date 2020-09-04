using Entities.Entities;

namespace Core.Interfaces
{
    public interface ILoginService
    {
        UsuarioLogin Login(string email, string password);
        UsuarioLogin LoginFacebook(string accessToken);
        string AlterarSenha(string email);
        bool AlterarSenha(AlteraSenha model);
    }
}