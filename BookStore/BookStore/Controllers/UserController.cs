using CommonLayer.Models;
using ManagerLayer.Interface;
using ManagerLayer.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RepositoryLayer.Entity;

namespace BookStore.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserManager userManager;
        private readonly IJwtTokenManager jwtTokenManager;

        public UserController(IUserManager userManager, IJwtTokenManager jwtTokenManager)
        {
            this.userManager = userManager;
            this.jwtTokenManager = jwtTokenManager;
           

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



        [HttpPost("login")]
        public IActionResult Login(LoginModel model)
        {
            var user = userManager.Login(model);

            if (user != null)
            {
                string token = jwtTokenManager.GenerateToken(user.Email, user.UserId, user.Role);

                return Ok(new
                {
                    Token = token,
                    Message = "Login successful"
                });
            }

            return Unauthorized(new { Message = "Invalid credentials" });
        }







    }
}
