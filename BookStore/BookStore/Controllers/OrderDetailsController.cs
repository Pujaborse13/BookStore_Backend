using System.Collections.Generic;
using System;
using ManagerLayer.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Context;
using RepositoryLayer.Helper;
using RepositoryLayer.Models;

namespace BookStore.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderDetailsController : ControllerBase
    {
        private readonly IOrderDetailsManager orderDetailsManager;
        private readonly JwtTokenHelper jwtTokenHelper;
        private readonly BookStoreDBContext context;


        public OrderDetailsController(IOrderDetailsManager orderDetailsManager, JwtTokenHelper jwtTokenHelper, BookStoreDBContext context)
        {
            this.jwtTokenHelper = jwtTokenHelper;
            this.context = context;
            this.orderDetailsManager = orderDetailsManager;

        }


        [HttpPost("placeorder")]
        public IActionResult PlaceOrder()
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                var orderResponse = orderDetailsManager.PlaceOrder(token);

                if (orderResponse == null || orderResponse.Orders.Count == 0)
                {
                    return BadRequest(new ResponseModel<string>
                    {
                        Success = false,
                        Message = "Could not place the order. Possible reasons: no items in cart, or user not found.",
                        Data = null
                    });
                }

                return Ok(new ResponseModel<List<OrderResponseModel>>
                {
                    Success = true,
                    Message = orderResponse.Message,
                    Data = orderResponse.Orders
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = "An unexpected error occurred.",
                    Data = ex.Message
                });
            }
        }



    }
}
