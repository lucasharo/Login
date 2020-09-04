using Dapper.Contrib.Extensions;

namespace Entities.Entities
{
    [Table("Usuario")]
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        public long IdFacebook { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
        public string TokenConfirmacao { get; set; }
        public string TokenAlterarSenha { get; set; }
    }
}
