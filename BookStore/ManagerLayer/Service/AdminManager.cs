using System;
using System.Collections.Generic;
using System.Text;
using CommonLayer.Models;
using ManagerLayer.Interface;
using Microsoft.Extensions.Configuration;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace ManagerLayer.Service
{
    public class AdminManager  : IAdminManager
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

    }
}
