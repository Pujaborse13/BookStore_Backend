using CommonLayer.Models;
using ManagerLayer.Interface;
using ManagerLayer.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Entity;

namespace BookStore.Controllers
{

    //[Route("api/[controller]")]
    //[ApiController]
    public class AdminController : ControllerBase
    {

        private readonly IAdminManager adminManager;
        private readonly IJwtTokenManager jwtTokenManager;

        public AdminController(IAdminManager adminManager, IJwtTokenManager jwtTokenManager)
        {
            this.adminManager = adminManager;
            this.jwtTokenManager = jwtTokenManager;
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
