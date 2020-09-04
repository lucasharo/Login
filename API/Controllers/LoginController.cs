using Microsoft.AspNetCore.Mvc;
using API.Helpers;
using Core.Interfaces;
using Entities.Entities;
using Microsoft.Extensions.Logging;
using API.Models;

namespace API.Controllers
{
    [ApiVersion("1")]
    public class LoginController : BaseController
    {
        private readonly ILoginService _service;

        public LoginController(ILogger<BaseController> logger, ILoginService service): base(logger)
        {
            _service = service;
        }

        [Consumes("application/x-www-form-urlencoded")]
        [HttpPost("Login")]
        public ActionResult<BaseResponse<UsuarioLogin>> Login([FromForm]Login login) => Execute(() => _service.Login(login.Email, login.Password));

        [HttpPost("LoginFacebook")]
        public ActionResult<BaseResponse<UsuarioLogin>> LoginFacebook([FromBody]LoginExternal login) => Execute(() => _service.LoginFacebook(login.AccessToken));

        [HttpGet("EsqueciMinhaSenha")]
        public ActionResult<BaseResponse<UsuarioLogin>> EsqueciMinhaSenha() => View("EsqueciMinhaSenha");

        [HttpPost("AlterarSenha")]
        public ActionResult<BaseResponse<UsuarioLogin>> AlterarSenha([FromBody]string email) => Execute(() => _service.AlterarSenha(email));

        [HttpGet("AlterarSenha")]
        public ActionResult<BaseResponse<UsuarioLogin>> AlterarSenha() => View("AlterarSenha");

        [Consumes("application/x-www-form-urlencoded")]
        [AuthorizeAlterarSenha]
        [HttpPut("AlterarSenha")]
        public ActionResult<BaseResponse<UsuarioLogin>> AlterarSenha([FromForm] AlteraSenha model) => Execute(() => _service.AlterarSenha(model));
    }
}