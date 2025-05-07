using System.Threading.Tasks;
using System;
using RepositoryLayer.Models;
using ManagerLayer.Interface;
using ManagerLayer.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RepositoryLayer.Entity;
using RepositoryLayer.Helper;
using Microsoft.AspNetCore.Authorization;

namespace BookStore.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserManager userManager;
        private readonly JwtTokenHelper jwtTokenHelper;

        public UserController(IUserManager userManager)
        {
            this.userManager = userManager;


        }


        //httplocal/api/Users/Reg
        [HttpPost]
        [Route("userRegistration")]
        public IActionResult Register(RegistrationModel model)
        {
            var check = userManager.CheckEmail(model.Email);

            if (check)
            {
                return BadRequest(new ResponseModel<UserEntity> { Success = false, Message = "Registration Fails " });
            }

            else
            {
                var result = userManager.Register(model);
      

                if (result != null)
                {
                    return Ok(new ResponseModel<UserEntity> { Success = true, Message = "Register Successfully", Data = result });

                }
                return BadRequest(new ResponseModel<UserEntity> { Success = false, Message = "Register Fail", Data = result });
            }

        }



        [HttpPost]
        [Route("userLogin")]
        public IActionResult Login(LoginModel model)
        {
            var user = userManager.Login(model);
            if (user != null)
            {
                return Ok(new ResponseModel<string> { Success = true, Message = "Login Successful", Data = user });
            }
            return BadRequest(new ResponseModel<string> { Success = false, Message = "Invalid Email or Password" });
        }


        
        [HttpPost]
        [Route("userForgotPassword")]
        public IActionResult ForgotPassword(string Email)
        {
            try
            {
                if (userManager.CheckEmail(Email))
                {
                    ForgotPasswordModel forgotPasswordModel = userManager.ForgotPassword(Email);

                    Send send = new Send();
                    send.SendMail(forgotPasswordModel.Email, forgotPasswordModel.Token);
                    return Ok(new ResponseModel<string> { Success = true, Message = "Mail send Sucessfully" });
                }
                else{

                    return BadRequest(new ResponseModel<string>() { Success = false, Message = "Email not send " });

                }
            }
            catch (Exception ex){
                throw ex;

            }


        }


       // [Authorize]
        [HttpPost]
        [Route("ResetPassword")]
        public ActionResult RestPassword(ResetPasswordModel reset){
            try
            {
                string Email = User.FindFirst("EmailID").Value;
                if (userManager.ResetPassword(Email, reset))
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
