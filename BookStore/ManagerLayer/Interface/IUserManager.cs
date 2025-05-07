using System;
using System.Collections.Generic;
using System.Text;
using RepositoryLayer.Models;
using RepositoryLayer.Entity;
namespace ManagerLayer.Interface
{
    public interface IUserManager
    {
        public UserEntity Register(RegistrationModel model);
        public bool CheckEmail(string email);
        public string Login(LoginModel model);
        
        public ForgotPasswordModel ForgotPassword(string Email);
        public bool ResetPassword(string Email, ResetPasswordModel resetPasswordModel);




    }
}
