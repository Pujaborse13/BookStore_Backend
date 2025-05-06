using CommonLayer.Models;
using ManagerLayer.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RepositoryLayer.Entity;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserManager userManager;

        public UserController(IUserManager userManager)
        {
            this.userManager = userManager;
           

        }


        //httplocal/api/Users/Reg
        [HttpPost]
        [Route("userRegistration")]
        public IActionResult Register(UserRegistrationModel model)
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




        [HttpPost("Login")]
        public IActionResult Login(LoginModel model)
        {
            var token = userManager.Login(model);
            if (token != null)
            {
                return Ok(new { Token = token, Message = "Login successful" });
            }
            return Unauthorized(new { Message = "Invalid credentials" });
        }








    }
}
