using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using RepositoryLayer.Models;
using ManagerLayer.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;

namespace ManagerLayer.Service
{
    public class AdminManager : IAdminManager
    {
        private readonly IAdminRepo adminRepo;
        private readonly IConfiguration configuration;

        public AdminManager(IAdminRepo adminRepo, IConfiguration configuration)
        {
            this.adminRepo = adminRepo;
            this.configuration = configuration;
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
            return adminRepo.Login(model);
        }

        public ForgotPasswordModel ForgotPassword(string Email)
        {
            return adminRepo.ForgotPassword(Email);
        }

        public bool ResetPassword(string Email, ResetPasswordModel resetPasswordModel)
        {
            return adminRepo.ResetPassword(Email, resetPasswordModel);
        }

    }
}
