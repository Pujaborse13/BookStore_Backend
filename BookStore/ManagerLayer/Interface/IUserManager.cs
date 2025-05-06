using System;
using System.Collections.Generic;
using System.Text;
using CommonLayer.Models;
using RepositoryLayer.Entity;
namespace ManagerLayer.Interface
{
    public interface IUserManager
    {
        public UserEntity Register(RegistrationModel model);
        public bool CheckEmail(string email);
        public string Login(LoginModel model);




    }
}
