using System.ComponentModel.DataAnnotations;

namespace Entities.Entities
{
    public class LoginExternal
    {
        [Required]
        public string AccessToken { get; set; }
    }
}