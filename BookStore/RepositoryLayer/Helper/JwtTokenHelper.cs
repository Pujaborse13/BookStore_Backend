using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Helper
{
    public class JwtTokenHelper
    {
        private readonly IUserRepo userRepo;
        private readonly IConfiguration configuration;


        public JwtTokenHelper(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public string GenerateToken(string email, int userId, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim("EmailID",email),
                new Claim("UserID",userId.ToString()),
                //new  Claim(ClaimTypes.Role, role),
                new Claim("role", role)
            };

            var token = new JwtSecurityToken(configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddHours(2), // Access token valid for 15 hours
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }


        public string ExtractRoleFromJwt(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
            
            Console.WriteLine($"Extracted role: {roleClaim}");

            if (string.IsNullOrEmpty(roleClaim))
            {
                throw new InvalidOperationException("Role not found in token.");
            }

            return roleClaim;
        }


    }
}
