using System;
using System.Collections.Generic;
using Core.Interfaces;
using Entities.Entities;
using Entities.Settings;
using Entities.Exceptions;
using Infra.Interfaces;
using Microsoft.AspNetCore.Http;
using Shared.Security;
using AutoMapper;

namespace Core.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public UsuarioService(ITokenService tokenService, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public UsuarioLogin CadastrarUsuario(Usuario usuario)
        {
            var userDB = _unitOfWork.UsuarioRepository.GetByEmail(usuario.Email);

            if (userDB != null)
            {
                throw new Exception("Usuário já cadastrado");
            }
            else if (string.IsNullOrEmpty(usuario.Password))
            {
                throw new Exception("Favor informar uma senha válida");
            }

            usuario.Role = Role.User;
            usuario.Password = Criptografia.SenhaCriptografada(usuario.Password);

            _unitOfWork.UsuarioRepository.Inserir(usuario);

            _unitOfWork.Commit();

            var usuarioLogin = _mapper.Map<UsuarioLogin>(usuario);

            return usuarioLogin;
        }

        public UsuarioLogin AlterarUsuario(int id, Usuario userAlterado)
        {
            var idToken = _tokenService.GetIdByToken();

            if (id != idToken && !_httpContextAccessor.HttpContext.User.IsInRole(Role.Admin))
            {
                throw new Exception("Id inválido");
            }

            var usuario = _unitOfWork.UsuarioRepository.Get(id);

            if(usuario == null)
            {
                throw new AppException("Usuário não encontrado");
            }
            else if (string.IsNullOrEmpty(userAlterado.Nome))
            {
                throw new Exception("Nome inválido");
            }
            else if (string.IsNullOrEmpty(userAlterado.Sobrenome))
            {
                throw new Exception("Sobrenome inválido");
            }

            if (_httpContextAccessor.HttpContext.User.IsInRole(Role.Admin))
            {
                if(userAlterado.Role == Role.Admin)
                {
                    usuario.Role = Role.Admin;
                }
                else
                {
                    usuario.Role = Role.User;
                }
            }

            usuario.Nome = userAlterado.Nome;
            usuario.Sobrenome = userAlterado.Sobrenome;

            if (id == idToken && !string.IsNullOrEmpty(userAlterado.Password))
            {
                usuario.Password = Criptografia.SenhaCriptografada(userAlterado.Password);
            }

            _unitOfWork.UsuarioRepository.Atualizar(usuario);

            _unitOfWork.Commit();

            var usuarioLogin = _mapper.Map<UsuarioLogin>(usuario);

            return usuarioLogin;
        }

        public IEnumerable<UsuarioLogin> Listar() => _mapper.Map<IEnumerable<UsuarioLogin>>(_unitOfWork.UsuarioRepository.Listar());

        public UsuarioLogin GetById(int id) => _mapper.Map<UsuarioLogin>(_unitOfWork.UsuarioRepository.Get(id));
    }
}