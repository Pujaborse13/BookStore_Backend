using System;
using System.Collections.Generic;
using System.Text;
using RepositoryLayer.Entity;
using RepositoryLayer.Models;


namespace RepositoryLayer.Interface
{
    public interface IUserRepo
    {
        public UserEntity Register(RegistrationModel model);
        public bool CheckEmail(string email);
        public string Login(LoginModel model);
        public ForgotPasswordModel ForgotPassword(string Email);   //Generates a password reset token.

       // public bool ResetPassword(string Email, ResetPasswordModel resetPasswordModel);


    }
}
