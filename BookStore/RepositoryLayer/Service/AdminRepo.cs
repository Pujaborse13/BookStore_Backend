using System;
using System.Linq;
using RepositoryLayer.Models;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using RepositoryLayer.Helper;
using Microsoft.Extensions.Configuration;

namespace RepositoryLayer.Service
{
    public class AdminRepo : IAdminRepo
    {
        private readonly BookStoreDBContext context;
       // private readonly IConfiguration configuration;
        private readonly JwtTokenHelper jwtTokenHelper;

        public AdminRepo(BookStoreDBContext context,   JwtTokenHelper jwtTokenHelper)
        {
            this.context = context;
            //this.configuration = configuration;
            this.jwtTokenHelper = jwtTokenHelper;
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

        //login
        public string Login(LoginModel model)
        {
            var checkUser = context.Admins.FirstOrDefault(x => x.Email == model.Email && x.Password == EncodePasswordToBase64(model.Password));
            if (checkUser != null)
            {
                var token = jwtTokenHelper.GenerateToken(checkUser.Email, checkUser.UserId, checkUser.Role);
                return token;
            }
            return null;

        }

        public ForgotPasswordModel ForgotPassword(string Email)
        {
            AdminEntity user = context.Admins.ToList().Find(user => user.Email == Email);

            ForgotPasswordModel forgotPassword = new ForgotPasswordModel();
            forgotPassword.Email = user.Email;
            forgotPassword.UserID = user.UserId;

            forgotPassword.Token = jwtTokenHelper.GenerateToken(user.Email, user.UserId, user.Role);

            return forgotPassword;
        }


        public bool ResetPassword(string Email, ResetPasswordModel resetPasswordModel)
        {
            AdminEntity User = context.Admins.ToList().Find(user => user.Email == Email);

            if (CheckEmail(User.Email))
            {

                User.Password = EncodePasswordToBase64(resetPasswordModel.ConfirmPassword);
                context.SaveChanges();
                return true;
            }

            else
            {
                return false;

            }

        }


    }
}
