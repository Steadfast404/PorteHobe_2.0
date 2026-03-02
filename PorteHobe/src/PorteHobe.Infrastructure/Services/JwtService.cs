using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Portehobe.Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Portehobe.Infrastructure.Services
{
    public interface IJwtService
    {
        string GenerateToken(AppUser user, IList<string> roles);  // ← Changed
    }

    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(AppUser user, IList<string> roles)  // ← Changed
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("FullName", user.FullName)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtKey = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is missing from configuration.");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"] ?? "PorteHobeAPI",
                audience: _config["Jwt:Audience"] ?? "PorteHobeClient",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:DurationInMinutes"] ?? "60")),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}