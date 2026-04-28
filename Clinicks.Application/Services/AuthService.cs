using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using Clinicks.Application.Context;
using Clinicks.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Clinicks.Application.Services
{
    public class AuthService
    {
        private readonly ClinicksDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(ClinicksDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<string?> IniciarSesion(string username, string password)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Nombre == username);

            // Verificamos si existe y si el hash de la clave coincide
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(password, usuario.Password))
                return null;

            return GenerarToken(usuario);
        }

        private string GenerarToken(Usuario usuario)
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