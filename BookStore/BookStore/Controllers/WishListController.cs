using System;
using System.Collections.Generic;
using ManagerLayer.Interface;
using ManagerLayer.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Models;

namespace BookStore.Controllers

{
    [ApiController]
    [Route("api/wishlist")]
    public class WishListController : ControllerBase
    {

        private readonly IWishListManager wishListManager;

        public WishListController(IWishListManager wishListManager)
        {
            this.wishListManager = wishListManager;
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddToWishList(int bookId)
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                var wishlistItem = wishListManager.AddToWishList(token, bookId);

                return Ok(new ResponseModel<WishListModel>
                {
                    Success = true,
                    Message = "Book added to wishlist successfully.",
                    Data = wishlistItem
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ResponseModel<string>{Success = false, 
                    Message = "You are not authorized: " + ex.Message});
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ResponseModel<string>{
                    Success = false,
                    Message = ex.Message});
            }
            catch (InvalidOperationException ex)
            {
                 // 409 Conflict
                return Conflict(new ResponseModel<string>{Success = false,Message = ex.Message});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string>{Success = false,Message = $"Internal server error: {ex.Message}"});
            }
        }




        [HttpGet]
        [Authorize]
        public IActionResult GetWishListDetails()
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (string.IsNullOrWhiteSpace(token))
                {
                    return Unauthorized(new ResponseModel<string>
                    {
                        Success = false,
                        Message = "Authorization token is missing.",
                        Data = null
                    });
                }

                var response = wishListManager.GetWishListDetails(token);
                if (!response.IsSuccess)
                {
                    if (response.Message.Contains("users"))
                    {
                        return Unauthorized(new ResponseModel<string> { Success = false, Message = response.Message });
                    }

                    // Handle empty wishlist as a valid case (200 OK)
                    if (response.Message.Contains("empty") || response.Message.Contains("not found"))
                        return Ok(new ResponseModel<WishListSummeryModel>
                        {
                            Success = true,
                            Message = "Your wishlist is empty.",
                            Data = new WishListSummeryModel
                            {
                                Items = new List<WishlListItemModel>(),
                                User = null 
                            }
                        });

                    return BadRequest(new ResponseModel<string> { Success = false, Message = response.Message });
                }
                return Ok(new ResponseModel<WishListSummeryModel>{Success = true,Message = response.Message,Data = response.Data});
            }

            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }




        [HttpDelete("{bookId}")]
        [Authorize]
        public IActionResult RemoveFromWishList(int bookId)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                //if (string.IsNullOrWhiteSpace(token))
                //{
                //    return Unauthorized(new ResponseModel<string>{
                //        Success = false,
                //        Message = "Authorization token is missing.",
                //        Data = null
                //    });
                //}

                var result = wishListManager.RemoveFromWishlist(token, bookId);

                /*
                if (result.Contains("Unauthorized"))
                {
                    return Unauthorized(new ResponseModel<string>
                    {
                        Success = false,
                        Message = result,
                        Data = null
                    });
                }


                if (result.Contains("not found"))
                {
                    return NotFound(new ResponseModel<string>{Success = false,Message = result,Data = null});
                }

                if (result.Contains("not zero"))
                {
                    return BadRequest(new ResponseModel<string>{Success = false,Message = result,Data = null});
                }

                return Ok(new ResponseModel<string>{Success = true,Message = result,Data = null});
            }*/

                //store procedure changes 

                return Ok(new ResponseModel<string>{Success = true,Message = result,Data = null});
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ResponseModel<string>{Success = false,Message = ex.Message,Data = null});
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ResponseModel<string>{Success = false,Message = ex.Message,Data = null});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Data = null
                });
            }
        }


    }
}