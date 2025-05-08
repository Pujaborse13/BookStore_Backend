using System;
using System.Collections.Generic;
using ManagerLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Helper;
using RepositoryLayer.Models;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BookController : Controller
    {

        private readonly IBookManager bookManager;
        private readonly JwtTokenHelper jwtTokenHelper;
        private readonly BookStoreDBContext context;


        public BookController(IBookManager bookManager, JwtTokenHelper jwtTokenHelper, BookStoreDBContext context)
        {
            this.bookManager = bookManager;
            this.jwtTokenHelper = jwtTokenHelper;
            this.context = context;

        }

        [HttpPost("loadfromcsv")]
        [Authorize]
        public IActionResult LoadBooksFromCsv()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = bookManager.LoadBooksFromCsv(token);
            if (result.StartsWith("Unauthorized"))
                return Unauthorized(result);

            return Ok(result);
        }


        [HttpGet("all")]
        [Authorize]
        public IActionResult GetAllBooks()
        {
            try
            {
                var books = bookManager.GetAllBooks();

                return Ok(new ResponseModel<List<BookEntity>>{Success = true,Message = "Books retrieved successfully",Data = books});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string>{Success = false,Message = $"Internal server error: {ex.Message}"});
            }
        }



        [HttpGet("{id}")]
        [Authorize] 
        public IActionResult GetBookById(int id)
        {
            try
            {
                var result = bookManager.GetBookById(id);

                if (result != null)
                {
                  return Ok(new ResponseModel<BookEntity> {Success = true,Message = "Book retrieved successfully", Data = result});
                }
                else
                {
                    return NotFound(new ResponseModel<BookEntity>{Success = false,Message = $"No book found with ID {id}",Data = null});
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string>{Success = false,Message = $"Internal server error: {ex.Message}"});
            }
        }




    }
}
