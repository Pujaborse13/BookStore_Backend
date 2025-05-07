using RepositoryLayer.Models;
using ManagerLayer.Interface;
using ManagerLayer.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Entity;
using RepositoryLayer.Helper;
using Microsoft.AspNetCore.Identity;

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



    }

   
}
