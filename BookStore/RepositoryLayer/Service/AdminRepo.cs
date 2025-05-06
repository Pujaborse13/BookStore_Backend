using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLayer.Models;
using Microsoft.Extensions.Configuration;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
    public class AdminRepo : IAdminRepo
    {
        private readonly BookStoreDBContext context;
        public AdminRepo(BookStoreDBContext context)
        {
            this.context = context;

        }

        public AdminEntity Register(RegistrationModel model)
        {

            AdminEntity admin = new AdminEntity();

            admin.FullName = model.FullName;
            admin.Email = model.Email;
            admin.MobileNumber = model.MobileNumber;
            admin.Password = EncodePasswordToBase64(model.Password); //Encoding the password into Base64 format for storage
            admin.Role = "admin";

            this.context.Admins.Add(admin);
            context.SaveChanges();
            return admin;

        }

        public bool CheckEmail(string email)
        {
            var result = this.context.Admins.FirstOrDefault(x => x.Email == email);
            return result != null;
        }

        private string EncodePasswordToBase64(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);

            }
        }

        public AdminEntity Login(LoginModel model)
        {
            var encodedPassword = EncodePasswordToBase64(model.Password);
            return context.Admins.FirstOrDefault(x => x.Email == model.Email && x.Password == encodedPassword);
        }
    }
}
