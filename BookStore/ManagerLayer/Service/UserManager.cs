using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using RepositoryLayer.Models;
using RepositoryLayer.Helper;
using ManagerLayer.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;


namespace ManagerLayer.Service
{
    public  class UserManager :IUserManager
    {
        private readonly IUserRepo userRepo;

        public UserManager(IUserRepo userRepo, BookStoreDBContext context)
        {
            this.userRepo = userRepo;
        }


        public UserEntity Register(RegistrationModel model)
        {
            return userRepo.Register(model);
        }

        public bool CheckEmail(string email)
        {
            return userRepo.CheckEmail(email);
        }


        public TokenResponse Login(LoginModel model)
        {
            return userRepo.Login(model);
        }

        public ForgotPasswordModel ForgotPassword(string Email)
        {
            return userRepo.ForgotPassword(Email);
        }


        public bool ResetPassword(string Email, ResetPasswordModel resetPasswordModel)
        {
            return userRepo.ResetPassword(Email, resetPasswordModel);

        }

    }
}
