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


                if (cartItem == null)
                    {
                        return BadRequest(new { message = "Could not add to cart. Possible reasons: invalid token, book/user not found, or only users have access admin can't add cart" });
                    }

                    return Ok(new
                    {
                        message = "Book added to cart successfully.", data = cartItem});
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = $"An unexpected error occurred: {ex.Message}" });
                }
            }





        }
    }
