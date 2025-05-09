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


        [HttpGet]
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


        [HttpPut("{id}")]
        [Authorize] 
        public IActionResult UpdateBook(int id, [FromBody] BookEntity updatedBook)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var role = jwtTokenHelper.ExtractRoleFromJwt(token);

                if (role == null || role.ToLower() != "admin")
                {
                    return Unauthorized(new ResponseModel<string>{Success = false,
                        Message = "Unauthorized: Only admins can update books"});
                }

                var result = bookManager.UpdateBookById(id, updatedBook);

                if (result == null)
                {
                    return NotFound(new ResponseModel<BookEntity>{Success = false,Message = $"No book found with ID {id}"});
                }

                return Ok(new ResponseModel<BookEntity>{Success = true,Message = "Book updated successfully",Data = result});
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





        [HttpPost("add")]
        [Authorize]
        public IActionResult AddBook([FromBody] BookEntity newBook)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var role = jwtTokenHelper.ExtractRoleFromJwt(token);

                if (role == null || role.ToLower() != "admin")
                {
                    return Unauthorized(new ResponseModel<string>{ Success = false,Message = "Only admins are allowed to add books"});
                }

                var result = bookManager.AddBook(newBook);

                return Ok(new ResponseModel<BookEntity>{Success = true,Message = "Book added successfully",Data = result});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string>{Success = false,Message = $"Internal server error: {ex.Message}"});
            }
        }



        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteBook(int id)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var role = jwtTokenHelper.ExtractRoleFromJwt(token);

                if (role == null || role.ToLower() != "admin")
                {
                    return Unauthorized(new ResponseModel<string>{Success = false,Message = "Only admins can delete books"});
                }

                var success = bookManager.DeleteBookById(id);

                if (!success)
                {
                    return NotFound(new ResponseModel<string>{Success = false, Message = $"No book found with ID {id}"});
                }

                return Ok(new ResponseModel<string>{Success = true,Message = "Book deleted successfully"});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string>{ Success = false,Message = $"Internal server error: {ex.Message}"});
            }
        }


        [HttpGet("price")]
        [Authorize]
        public IActionResult GetBooksSortedByPrice([FromQuery] string order = "asc")
        {
            try
            {
                var sortedBooks = bookManager.GetBooksSortedByPrice(order);

                return Ok(new ResponseModel<List<BookEntity>> { Success = true, Message = $"Books sorted by price ({order})", Data = sortedBooks });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string> { Success = false, Message = $"Internal server error: {ex.Message}" });
            }
        }




        [HttpGet("searchbyauthor")]
        [Authorize]
        public IActionResult SearchBooksByAuthor([FromQuery] string author)
        {
            try
            {
                var books = bookManager.SearchBooksByAuthor(author);

                return Ok(new ResponseModel<List<BookEntity>>{Success = true,Message = $"Books by author: '{author}'",Data = books
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string>{Success = false,Message = $"Internal server error: {ex.Message}"});
            }
        }


        [HttpGet("sortbookbypriceasc")]
        [Authorize]
        public IActionResult GetBooksByPriceAscending()
        {
            try
            {
                var books = bookManager.SortBooksByPriceAscending();

                return Ok(new ResponseModel<List<BookEntity>>{Success = true,
                        Message = "Books sorted by price in ascending order",
                    Data = books});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string>{Success = false,
                    Message = $"Internal server error: {ex.Message}"});
            }
        }





    }
}
