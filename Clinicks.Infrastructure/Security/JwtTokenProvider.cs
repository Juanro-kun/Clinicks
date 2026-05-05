using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Clinicks.Application.Interfaces;
using Clinicks.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Clinicks.Infrastructure.Security
{
    public class JwtTokenProvider : ITokenProvider
    {
        private readonly IConfiguration _config;

        public JwtTokenProvider(IConfiguration config)
        {
            _config = config;
        }

        public string GenerarToken(Usuario usuario)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim("UsuarioId", usuario.UsuarioId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
