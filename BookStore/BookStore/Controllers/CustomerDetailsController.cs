using System;
using System.Collections.Generic;
using ManagerLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Models;

namespace BookStore.Controllers
{
    [Route("api/customer")]
    [ApiController]
    public class CustomerDetailsController : ControllerBase
    {

        private readonly ICustomerDetailsManager customerDetailsManager;


        public CustomerDetailsController(ICustomerDetailsManager customerDetailsManager)
        {
            this.customerDetailsManager = customerDetailsManager;


        }


        [HttpPost]
        [Authorize]

        public IActionResult AddCustomerDetails([FromBody] CustomerDetailsModel model)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                var result = customerDetailsManager.AddCustomerDetails(model, token);

                return Ok(new ResponseModel<CustomerDetailsModel>{
                    Success = true,Message = "Customer details added successfully.",Data = result});
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ResponseModel<string>{
                    Success = false,Message = ex.Message,Data = null});
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<string>
                {
                    Success = false,Message = "Customer details not added.",Data = ex.Message});
            }
        }

        [HttpGet]
        [Authorize]

        public IActionResult GetAllCustomerDetails()
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                var result = customerDetailsManager.GetAllCustomerDetails(token);

                return Ok(new ResponseModel<IEnumerable<CustomerDetailsResponseModel>>
                {
                    Success = true,Message = "Customer details fetched successfully.",Data = result});
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ResponseModel<string>
                {
                    Success = false,Message = ex.Message,Data = null});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,Message = "Something went wrong.",Data = ex.Message});
            }
        }






    }
}
