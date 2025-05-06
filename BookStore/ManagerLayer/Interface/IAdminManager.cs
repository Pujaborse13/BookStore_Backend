using System;
using System.Collections.Generic;
using System.Text;
using CommonLayer.Models;
using RepositoryLayer.Entity;

namespace ManagerLayer.Interface
{
    public interface IAdminManager 
    {
        public AdminEntity Register(RegistrationModel model);
        public bool CheckEmail(string email);
        public AdminEntity Login(LoginModel model);

    }
}
