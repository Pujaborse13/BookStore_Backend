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
        private readonly IJwtTokenManager jwtTokenManager;


        public UserManager(IUserRepo userRepo, IConfiguration configuration, IJwtTokenManager jwtTokenManager)
        {
            this.userRepo = userRepo;
            this.configuration = configuration;
            this.jwtTokenManager = jwtTokenManager;
        }


        public UserEntity Register(RegistrationModel model)
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
                return jwtTokenManager.GenerateToken(user.Email, user.UserId, user.Role);
            }
            return null;
        }


      
    }
}
