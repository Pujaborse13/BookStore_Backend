using System;
using System.Collections.Generic;
using System.Text;
using RepositoryLayer.Models;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface IAdminRepo
    {
        public AdminEntity Register(RegistrationModel model);
        public bool CheckEmail(string email);
        public string Login(LoginModel model);
        public ForgotPasswordModel ForgotPassword(string Email);   //Generates a password reset token.

    }
}
