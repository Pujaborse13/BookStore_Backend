using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using CommonLayer.Models;
using System.Linq;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Data;



namespace RepositoryLayer.Service
{
    public class UserRepo : IUserRepo
    {
        private readonly BookStoreDBContext context;
        private readonly IConfiguration configuration;


        public UserRepo(BookStoreDBContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;  

        }


        public UserEntity Register(UserRegistrationModel model)
        {

            UserEntity user = new UserEntity();

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.MobileNumber = model.MobileNumber;
            user.Password = EncodePasswordToBase64(model.Password); //Encoding the password into Base64 format for storage
            user.Role = "user";

            this.context.Users.Add(user);
            context.SaveChanges();
            return user;

        }

        public bool CheckEmail(string email)
        {
            var result = this.context.Users.FirstOrDefault(x => x.Email == email);
            if (result == null)
            {
                return false;
            }

            return true;
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

        public UserEntity Login(LoginModel model)
        {
            var encodedPassword = EncodePasswordToBase64(model.Password);
            return context.Users.FirstOrDefault(x => x.Email == model.Email && x.Password == encodedPassword);
        }
    }
}
