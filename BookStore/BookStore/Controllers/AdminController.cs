using RepositoryLayer.Models;
using ManagerLayer.Interface;
using ManagerLayer.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Entity;
using RepositoryLayer.Helper;
using Microsoft.AspNetCore.Identity;
using System;
using Microsoft.AspNetCore.Authorization;

namespace BookStore.Controllers
{

    //[Route("api/[controller]")]
    //[ApiController]
    public class AdminController : ControllerBase
    {

        private readonly IAdminManager adminManager;

        public AdminController(IAdminManager adminManager)
        {
            this.adminManager = adminManager;
        }


        //httplocal/api/Users/Reg
        [HttpPost]
        [Route("adminRegistration")]
        public IActionResult Register(RegistrationModel model)
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


        [HttpPost("adminLogin")]
        public IActionResult Login(LoginModel model)
        {


            var user = adminManager.Login(model);
            if (user != null)
            {
                return Ok(new ResponseModel<string> { Success = true, Message = "Login Successful", Data = user });
            }
            return BadRequest(new ResponseModel<string> { Success = false, Message = "Invalid Email or Password" });

        }



        [HttpPost]
        [Route("adminForgotPassword")]
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
                throw ex;

            }


        }



        [HttpPost]
        [Route("adminResetPassword")]
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
                throw ex;
            }


        }







    }

   
}
