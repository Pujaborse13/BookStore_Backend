using System;
using System.Collections.Generic;
using System.Text;
using RepositoryLayer.Entity;
using CommonLayer.Models;


namespace RepositoryLayer.Interface
{
    public interface IUserRepo
    {
        public UserEntity Register(RegistrationModel model);
        public bool CheckEmail(string email);
        public UserEntity Login(LoginModel model);


    }
}
