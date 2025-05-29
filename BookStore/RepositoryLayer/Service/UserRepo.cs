using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using RepositoryLayer.Models;
using System.Linq;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Data;
using RepositoryLayer.Helper;
using Microsoft.EntityFrameworkCore;
//using ManagerLayer.Interface;



namespace RepositoryLayer.Service
{
    public class UserRepo : IUserRepo
    {
        private readonly BookStoreDBContext context;
        private readonly IConfiguration configuration;
        private readonly JwtTokenHelper jwtTokenHelper;



        public UserRepo(BookStoreDBContext context, IConfiguration configuration, JwtTokenHelper jwtTokenHelper)
        {
            this.context = context;
            this.configuration = configuration; 
            this.jwtTokenHelper = jwtTokenHelper;

        }


        //Registration 
        public UserEntity Register(RegistrationModel model)
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

        //check Email
        public bool CheckEmail(string email)
        {
            var result = this.context.Users.FirstOrDefault(x => x.Email == email);
            if (result == null)
            {
                return false;
            }

            return true;
        }

        //encode password 
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
        public TokenResponse Login(LoginModel model)
        {
            var checkUser = context.Users.FirstOrDefault(x => x.Email == model.Email && x.Password == EncodePasswordToBase64(model.Password));
            if (checkUser != null)
            {
                var accessToken = jwtTokenHelper.GenerateToken(checkUser.Email, checkUser.UserId , checkUser.Role);
                var refreshToken = jwtTokenHelper.GenerateRefreshToken();

                checkUser.RefreshToken = refreshToken;
                checkUser.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
         
                context.SaveChanges();

                return new TokenResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    FullName = checkUser.FullName
                };
                
            }
            return null;
        }



        //forgot password
        public ForgotPasswordModel ForgotPassword(string Email)
        {
            UserEntity user = context.Users.ToList().Find(user => user.Email == Email);

            ForgotPasswordModel forgotPassword = new ForgotPasswordModel();
            forgotPassword.Email = user.Email;
            forgotPassword.UserID = user.UserId;

            forgotPassword.Token = jwtTokenHelper.GenerateToken(user.Email, user.UserId, user.Role);

            return forgotPassword;

        }

        //Reset Password
        public bool ResetPassword(string Email, ResetPasswordModel resetPasswordModel)
        {
            UserEntity User = context.Users.ToList().Find(user => user.Email == Email);

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





