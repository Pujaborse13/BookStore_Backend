using System;
using ManagerLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Context;
using RepositoryLayer.Helper;
using RepositoryLayer.Models;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartManager cartManager;
        private readonly JwtTokenHelper jwtTokenHelper;
        private readonly BookStoreDBContext context;


        public CartController(ICartManager cartManager, JwtTokenHelper jwtTokenHelper, BookStoreDBContext context)
        {
            this.jwtTokenHelper = jwtTokenHelper;
            this.context = context;
            this.cartManager = cartManager;

        }


            [HttpPost]
            public IActionResult AddToCart(int bookId)
            {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (string.IsNullOrWhiteSpace(token))
                {
                    return Unauthorized(new { message = "Token is missing or invalid" });
                }


                var cartItem = cartManager.AddToCart(token, bookId);
                return Ok(new
                {
                    message = "Book added to cart successfully.", data = cartItem });
                }

            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message); 
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message }); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An unexpected error occurred: {ex.Message}" });
            }
            }


        [HttpGet]
        public IActionResult GetCart()
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized(new ResponseModel<string>{Success = false,Message = "Authorization token is missing.",Data = null});
                }

                var response = cartManager.GetCartDetails(token);

                if (!response.IsSuccess)
                {
                    if (response.Message.Contains("users"))
                    {
                        return Unauthorized(new ResponseModel<string>{Success = false,Message = response.Message,Data = null});
                    }

                    else if (response.Message.Contains("empty") || response.Message.Contains("not found"))
                    {
                        return NotFound(new ResponseModel<string>{Success = false,Message = response.Message,Data = null});
                    }

                    else
                    {
                        return BadRequest(new ResponseModel<string>{Success = false,Message = response.Message,Data = null});
                    }
                }

                return Ok(new ResponseModel<CartSummeryModel>{Success = true,Message = response.Message,Data = response.Data});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string>{Success = false,Message = $"Internal server error: {ex.Message}",Data = null});
            }
        }


        [HttpPut("updatequantity")]
        [Authorize]
        public IActionResult UpdateCartQuantity(int bookId, string action)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (string.IsNullOrWhiteSpace(token))
                    return Unauthorized(new ResponseModel<string>{Success = false,Message = "Authorization token is missing."});

                var result = cartManager.UpdateCartQuantity(token, bookId, action);

                if (result == null)
                {
                    return Ok(new ResponseModel<string>{Success = true,Message = "Cart item removed as quantity became zero."});
                }

                return Ok(new ResponseModel<CartModel>{Success = true,Message = $"Cart updated successfully",Data = result});
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ResponseModel<string>{Success = false,Message = ex.Message});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string>{Success = false,Message = $"Internal server error: {ex.Message}"});
            }
        }


        [HttpDelete("{bookId}")]
        public IActionResult DeleteCartItem(int bookId)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                    return Unauthorized("Authorization token is missing.");

                var result = cartManager.DeleteFromCartIfQuantityZero(token, bookId);

                if (result.Contains("Unauthorized"))
                    return Unauthorized(result);
                else if (result.Contains("not found"))
                    return NotFound(result);
                else if (result.Contains("not zero"))
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }




    }
}
