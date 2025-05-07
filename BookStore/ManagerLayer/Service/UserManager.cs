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
        private readonly IConfiguration configuration;
         private readonly IUserManager userManager;
        private readonly BookStoreDBContext context;




        public UserManager(IUserRepo userRepo, IConfiguration configuration, BookStoreDBContext context)
        {
            this.userRepo = userRepo;
            this.configuration = configuration;
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
            return userRepo.Login(model);
        }




        //public ForgotPasswordModel ForgotPassword(string Email)
        //{
        //    UserEntity user = context.Users.ToList().Find(user => user.Email == Email);
        //    ForgotPasswordModel forgotPassword = new ForgotPasswordModel();
        //    forgotPassword.Email = user.Email;
        //    forgotPassword.UserID = user.UserId;
        //    forgotPassword.Token = jwtTokenManager.GenerateToken(user.Email, user.UserId, user.Role);

        //    return forgotPassword;


        //}


    }
}
