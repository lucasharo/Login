using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Core.Interfaces;
using Entities.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.Linq;
using Entities.Settings;

namespace Core.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GenerateToken(Usuario user)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("Id", user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(240),
                SigningCredentials = new SigningCredentials(new X509SecurityKey(new X509Certificate2(_configuration.GetSection("Certificado:Diretorio").Value, _configuration.GetSection("Certificado:Senha").Value)), SecurityAlgorithms.RsaSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public string GenerateTokenChangePassword(long id)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("Id", id.ToString()),
                    new Claim(ClaimTypes.Role, Role.AlterarSenha)
                }),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(new X509SecurityKey(new X509Certificate2(_configuration.GetSection("Certificado:Diretorio").Value, _configuration.GetSection("Certificado:Senha").Value)), SecurityAlgorithms.RsaSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public string GetToken()
        {
            return _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Authorization].FirstOrDefault().Replace("Bearer", "").Trim();
        }

        public int GetIdByToken()
        {
            var id = _httpContextAccessor.HttpContext.User?.Claims.FirstOrDefault(c => c.Type == "Id");

            if (id != null)
            {
                return Convert.ToInt32(id.Value);
            }
            else
            {
                throw new Exception("Usuário não identificado no token");
            }
        }
    }
}