using System;
using System.Collections.Generic;
using System.Text;
using CommonLayer.Models;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface IAdminRepo
    {
        public AdminEntity Register(RegistrationModel model);
        public bool CheckEmail(string email);
        public AdminEntity Login(LoginModel model); 
    }
}
