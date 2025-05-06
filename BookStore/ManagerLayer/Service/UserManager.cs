using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CommonLayer.Models;
using ManagerLayer.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace ManagerLayer.Service
{
    public  class UserManager :IUserManager
    {
        private readonly IUserRepo userRepo;
        private readonly IConfiguration configuration;


        public UserManager(IUserRepo userRepo, IConfiguration configuration)
        {
            this.userRepo = userRepo;
            this.configuration = configuration;
        }


        public UserEntity Register(UserRegistrationModel model)
        {
            return userRepo.Register(model);
        }

        public bool CheckEmail(string email)
        {
            return userRepo.CheckEmail(email);
        }

       

        public string Login(LoginModel model)
        {
            var user = userRepo.Login(model);
            if (user != null)
            {
                return GenerateToken(user.Email, user.UserId, user.Role);
            }
            return null;
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
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
