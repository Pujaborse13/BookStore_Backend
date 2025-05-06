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
using RepositoryLayer.Service;

namespace ManagerLayer.Service
{
    public class AdminManager  : IAdminManager
    {
        private readonly IAdminRepo adminRepo;
        private readonly IConfiguration configuration;
        private readonly IJwtTokenManager jwtTokenManager;


        public AdminManager(IAdminRepo adminRepo, IConfiguration configuration, IJwtTokenManager jwtTokenManager)
        {
            this.adminRepo = adminRepo;
            this.configuration = configuration;
            this.jwtTokenManager = jwtTokenManager;
        }


        public AdminEntity Register(RegistrationModel model)
        {
            return adminRepo.Register(model);
        }

        public bool CheckEmail(string email)
        {
            return adminRepo.CheckEmail(email);
        }




        public string Login(LoginModel model)
        {
            var user = adminRepo.Login(model);
            if (user != null)
            {
                return jwtTokenManager.GenerateToken(user.Email, user.UserId, user.Role);
            }
            return null;
        }


        
    }
}
