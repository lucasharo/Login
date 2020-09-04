using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using Core.Interfaces;
using Entities.Entities;
using Entities.Settings;
using System.Text;
using Entities.Exceptions;
using Infra.Interfaces;
using Shared.Security;
using AutoMapper;

namespace Core.Services
{
    public class LoginService : ILoginService
    {
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly FacebookAuthSettings _fbAuthSettings;

        private static readonly HttpClient _client = new HttpClient();

        public LoginService(IConfiguration configuration, ITokenService tokenService, IEmailService emailService, IUnitOfWork unitOfWork, IMapper mapper, IOptions<FacebookAuthSettings> fbAuthSettingsAccessor)
        {
            _configuration = configuration;
            _tokenService = tokenService;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fbAuthSettings = fbAuthSettingsAccessor.Value;
        }

        public UsuarioLogin Login(string email, string password)
        {
            var usuario = _unitOfWork.UsuarioRepository.GetByEmail(email);

            if (usuario == null)
            {
                throw new AppException("Usuário não encontrado");
            }
            else if (usuario.Password != Criptografia.SenhaCriptografada(password))
            {
                throw new AppException("Senha inválida");
            }

            usuario.Token = _tokenService.GenerateToken(usuario);

            _unitOfWork.UsuarioRepository.Atualizar(usuario);

            _unitOfWork.Commit();

            var usuarioLogin = _mapper.Map<UsuarioLogin>(usuario);

            return usuarioLogin;
        }

        public UsuarioLogin LoginFacebook(string accessToken)
        {
            var appAccessTokenResponse = _client.GetStringAsync($"https://graph.facebook.com/oauth/access_token?client_id={_fbAuthSettings.AppId}&client_secret={_fbAuthSettings.AppSecret}&grant_type=client_credentials").Result;
            var appAccessToken = JsonConvert.DeserializeObject<FacebookAppAccessToken>(appAccessTokenResponse);

            var userAccessTokenValidationResponse = _client.GetStringAsync($"https://graph.facebook.com/debug_token?input_token={accessToken}&access_token={appAccessToken.AccessToken}").Result;
            var userAccessTokenValidation = JsonConvert.DeserializeObject<FacebookUserAccessTokenValidation>(userAccessTokenValidationResponse);

            if (!userAccessTokenValidation.Data.IsValid)
            {
                throw new Exception("Token inválido");
            }

            var userInfoResponse = _client.GetStringAsync($"https://graph.facebook.com/v2.8/me?fields=id,email,first_name,last_name,name,gender,locale,birthday,picture&access_token={accessToken}").Result;
            var userInfo = JsonConvert.DeserializeObject<FacebookUserData>(userInfoResponse);

            var usuario = _unitOfWork.UsuarioRepository.GetByIdFacebook(userInfo.Id);

            if (usuario == null)
            {
                usuario = new Usuario
                {
                    IdFacebook = userInfo.Id,
                    Email = userInfo.Email,
                    Nome = userInfo.FirstName,
                    Sobrenome = userInfo.LastName,
                    Role = Role.User
                };

                _unitOfWork.UsuarioRepository.Inserir(usuario);
            }

            if (usuario == null)
            {
                throw new Exception("Erro ao cadastrar usuário");
            }

            usuario.Token = _tokenService.GenerateToken(usuario);

            _unitOfWork.UsuarioRepository.Atualizar(usuario);

            _unitOfWork.Commit();

            var usuarioLogin = _mapper.Map<UsuarioLogin>(usuario);

            return usuarioLogin;
        }

        public string AlterarSenha(string email)
        {
            var usuario = _unitOfWork.UsuarioRepository.GetByEmail(email);

            if (usuario == null)
            {
                throw new AppException("Usuário não encontrado");
            }

            usuario.TokenAlterarSenha = _tokenService.GenerateTokenChangePassword(usuario.Id);

            var url = "https://localhost:44312/api/v1/Login/AlterarSenha?token=" + usuario.TokenAlterarSenha;

            _emailService.SendEmail(usuario.Email, "Alteração de senha", url);

            _unitOfWork.UsuarioRepository.Atualizar(usuario);

            _unitOfWork.Commit();

            return "Favor ferificar seu e-mail para alterar a senha";
        }

        public bool AlterarSenha(AlteraSenha model)
        {
            var id = _tokenService.GetIdByToken();

            var usuario = _unitOfWork.UsuarioRepository.Get(id);

            if (usuario != null)
            {
                var token = _tokenService.GetToken();

                if (usuario.TokenAlterarSenha == token)
                {
                    if (model.Password == model.ConfirmPassword)
                    {
                        usuario.Password = Criptografia.SenhaCriptografada(model.Password);
                        usuario.TokenAlterarSenha = null;

                        _unitOfWork.UsuarioRepository.Atualizar(usuario);

                        _unitOfWork.Commit();

                        return true;
                    }
                    else
                    {
                        throw new Exception("Senha inválida");
                    }
                }
                else
                {
                    throw new Exception("Token inválido");
                }
            }
            else
            {
                throw new AppException("Usuário não encontrado");
            }
        }
    }
}