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
using RepositoryLayer.Context;
using System.Linq;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {

        private readonly IUserManager userManager;
        private readonly JwtTokenHelper jwtTokenHelper;
        private readonly BookStoreDBContext context;
        private readonly RabbitMQProducer rabbitMQProducer;
        private readonly ILogger<UserController> logger;



        public UserController(IUserManager userManager, JwtTokenHelper jwtTokenHelper , BookStoreDBContext context, RabbitMQProducer rabbitMQProducer, ILogger<UserController> logger)
        {
            this.userManager = userManager;
            this.jwtTokenHelper = jwtTokenHelper;
            this.context = context;
            this.rabbitMQProducer = rabbitMQProducer;
            this.logger = logger;


        }


        [HttpPost]
        public IActionResult Register([FromBody] RegistrationModel model)
        {
            try
            {
                logger.LogInformation("Register endpoint called for Email: {Email}", model.Email);

                var check = userManager.CheckEmail(model.Email);

                if (check)
                {
                    logger.LogWarning("Registration failed: Email already exists - {Email}", model.Email);
                    return BadRequest(new ResponseModel<UserEntity> { Success = false, Message = "Registration Fails " });
                }

                else
                {
                    var result = userManager.Register(model);

                    HttpContext.Session.SetInt32("UserId", result.UserId); // set Session , logger
                  
                    if (result != null)
                    {

                        logger.LogInformation("User registered successfully: {Email}", model.Email);
                        return Ok(new ResponseModel<UserEntity> { Success = true, Message = "Register Successfully", Data = result });

                    }
                    logger.LogError("User registration returned null for Email: {Email}", model.Email);
                    return BadRequest(new ResponseModel<UserEntity> { Success = false, Message = "Register Fail", Data = result });
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception occurred during registration for Email: {Email}", model.Email);
                return StatusCode(500, new ResponseModel<string> { Success = false, Message = $"Internal server error: {ex.Message}" });

            }

        }


        [HttpPost("login")]

        public IActionResult Login(LoginModel model)
        {
            var tokenResponse = userManager.Login(model);

            if (tokenResponse != null)
            {
                return Ok(new ResponseModel<TokenResponse> { Success = true, Message = "Login Successful", Data = tokenResponse });
            }
            return BadRequest(new ResponseModel<string> { Success = false, Message = "Invalid Email or Password" });
        }



        /*
          [HttpPost("forgot-password")]
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
                  return StatusCode(500, new ResponseModel<string> { Success = false, Message = $"Internal server error: {ex.Message}" });

              }
          }
        */
        
        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword(string Email)
        {
            try
            {
                if (userManager.CheckEmail(Email))
                {
                    ForgotPasswordModel forgotPasswordModel = userManager.ForgotPassword(Email);

                    //Create RabbitMQ Email Message
                    var message = new RabbitMQEmailModel
                    {
                        ToEmail = forgotPasswordModel.Email,
                        Subject = "Forgot Password",
                        Body = $"Your password reset token is: {forgotPasswordModel.Token}"
                    };
                    

                    rabbitMQProducer.SendMessage(message);

                    //Send send = new Send();
                    //send.SendMail(forgotPasswordModel.Email, forgotPasswordModel.Token);
                    return Ok(new ResponseModel<string> { Success = true, Message = "Mail send Sucessfully" });
                }
                else
                {

                    return BadRequest(new ResponseModel<string>() { Success = false, Message = "Email not send " });

                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string> { Success = false, Message = $"Internal server error: {ex.Message}" });

            }


        }


        [HttpPost("reset-password")]
        public ActionResult RestPassword(ResetPasswordModel reset)
        {
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
                return StatusCode(500, new ResponseModel<string> { Success = false, Message = $"Login failed: {ex.Message}" });

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

                var user = context.Users.FirstOrDefault(u => u.RefreshToken == request.RefreshToken);

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
