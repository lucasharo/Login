using Entities.Entities;

namespace Core.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(Usuario user);
        string GenerateTokenChangePassword(long id);
        string GetToken();
        int GetIdByToken();
    }
}