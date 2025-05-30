﻿using RepositoryLayer.Models;
using ManagerLayer.Interface;
using ManagerLayer.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Entity;
using RepositoryLayer.Helper;
using Microsoft.AspNetCore.Identity;
using System;
using Microsoft.AspNetCore.Authorization;
using RepositoryLayer.Context;
using System.Linq;

namespace BookStore.Controllers
{

    [ApiController]
    [Route("api/admins")]
    public class AdminController : ControllerBase
    {

        private readonly IAdminManager adminManager;
        private readonly BookStoreDBContext context;
        private readonly JwtTokenHelper jwtTokenHelper;



        public AdminController(IAdminManager adminManager,BookStoreDBContext context, JwtTokenHelper jwtTokenHelper)
        {
            this.adminManager = adminManager;
            this.context = context;
            this.jwtTokenHelper = jwtTokenHelper;
        }



        [HttpPost]
        public IActionResult Register([FromBody] RegistrationModel model)
        {
            try
            {
                var check = adminManager.CheckEmail(model.Email);

                if (check)
                {
                    return BadRequest(new ResponseModel<UserEntity> { Success = false, Message = "Registration Fails " });
                }

                else
                {
                    var result = adminManager.Register(model);

                    if (result != null)
                    {
                        return Ok(new ResponseModel<AdminEntity> { Success = true, Message = "Register Successfully", Data = result });

                    }
                    return BadRequest(new ResponseModel<AdminEntity> { Success = false, Message = "Register Fail", Data = result });

                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string> { Success = false, Message = $"Error refreshing token: {ex.Message}" });

            }

        }
            
        
        [HttpPost("login")]
        public IActionResult Login(LoginModel model)
        {
            try
            {
                var user = adminManager.Login(model);
                if (user != null)
                {
                    return Ok(new ResponseModel<string> { Success = true, Message = "Login Successful", Data = user });
                }
                return BadRequest(new ResponseModel<string> { Success = false, Message = "Invalid Email or Password" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string> { Success = false, Message = $"Error refreshing token: {ex.Message}" });
            }

        }



        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword(string Email)
        {
            try
            {
                if (adminManager.CheckEmail(Email))
                {
                    ForgotPasswordModel forgotPasswordModel = adminManager.ForgotPassword(Email);

                    Send send = new Send();
                    send.SendMail(forgotPasswordModel.Email, forgotPasswordModel.Token);
                    return Ok(new ResponseModel<string> { Success = true, Message = "Mail send Sucessfully" });
                }
                else
                {
                    return BadRequest(new ResponseModel<string>() { Success = false, Message = "Email not send " });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string> { Success = false, Message = $"Error refreshing token: {ex.Message}" });
            }


        }



     
        [HttpPost("reset-password")]
        public ActionResult RestPassword(ResetPasswordModel reset)
        {
            try
            {
                string Email = User.FindFirst("EmailID").Value;
                if (adminManager.ResetPassword(Email, reset))
                {
                    return Ok(new ResponseModel<string> { Success = true, Message = "Password Changed" });
                }
                else
                {
                    return BadRequest(new ResponseModel<string> { Success = false, Message = "Password Wrong" });
                }
            }

            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string> { Success = false, Message = $"Error refreshing token: {ex.Message}" });
            }


        }


        [HttpPost("login-acref")]
        public IActionResult LoginRefereshToken(LoginModel model)
        {
            try {
                var tokenResponse = adminManager.LoginRefereshToken(model);

                if (tokenResponse != null)
                {
                    return Ok(new ResponseModel<TokenResponse> { Success = true, Message = "Login Successful", Data = tokenResponse });
                }
                return BadRequest(new ResponseModel<string> { Success = false, Message = "Invalid Email or Password" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string> { Success = false, Message = $"Error refreshing token: {ex.Message}" });
            }
        }



        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] RefreshTokenRequestModel request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.RefreshToken))
                {
                    return BadRequest(new { message = "Invalid client request" });
                }

                var user = context.Admins.FirstOrDefault(u => u.RefreshToken == request.RefreshToken);

                if (user == null || user.RefreshTokenExpiryTime <= DateTime.Now)
                {
                    return Unauthorized(new { message = "Invalid refresh token" });
                }

                var newAccessToken = jwtTokenHelper.GenerateToken(user.Email, user.UserId, user.Role);
                var newRefreshToken = jwtTokenHelper.GenerateRefreshToken();

                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
                context.SaveChanges();

                return Ok(new
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                });
            }

            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string> { Success = false, Message = $"Error refreshing token: {ex.Message}" });
            }


        }

    }

   
}
