using Microsoft.AspNetCore.Mvc;
using API.Helpers;
using Core.Interfaces;
using Entities.Entities;
using Microsoft.Extensions.Logging;
using API.Models;
using System.Collections.Generic;

namespace API.Controllers
{
    [ApiVersion("1")]
    public class UsuarioController : BaseController
    {
        private readonly IUsuarioService _userService;

        public UsuarioController(ILogger<BaseController> logger, IUsuarioService userService): base(logger)
        {
            _userService = userService;
        }

        [HttpPost]
        public ActionResult<BaseResponse<UsuarioLogin>> CadastrarUsuario([FromBody]Usuario usuario) => Execute(() => _userService.CadastrarUsuario(usuario));

        [AuthorizeUser]
        [HttpPut("{id}")]
        public ActionResult<BaseResponse<UsuarioLogin>> AlterarUsuario(int id, [FromBody]Usuario usuario) => Execute(() => _userService.AlterarUsuario(id, usuario));

        [AuthorizeAdmin]
        [HttpGet]
        public ActionResult<BaseResponse<IEnumerable<UsuarioLogin>>> GetAll() => Execute(() => _userService.Listar());

        [AuthorizeAdmin]
        [HttpGet("{id}")]
        public ActionResult<BaseResponse<UsuarioLogin>> GetById(int id) => Execute(() => _userService.GetById(id));
    }
}