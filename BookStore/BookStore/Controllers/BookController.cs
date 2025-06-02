using System;
using System.Collections.Generic;
using ManagerLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Helper;
using RepositoryLayer.Models;
using static System.Reflection.Metadata.BlobBuilder;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BookController : Controller
    {

        private readonly IBookManager bookManager;
        private readonly JwtTokenHelper jwtTokenHelper;
        private readonly BookStoreDBContext context;
        private readonly ILogger<BookController> logger;




        public BookController(IBookManager bookManager, JwtTokenHelper jwtTokenHelper, 
                                BookStoreDBContext context, ILogger<BookController> logger)
        {
            this.bookManager = bookManager;
            this.jwtTokenHelper = jwtTokenHelper;
            this.context = context;
            this.logger = logger;
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
                logger.LogInformation("Fetching all books...");
                var books = bookManager.GetAllBooks();

                logger.LogInformation("Books fetched successfully. Count: {Count}", books.Count);
                return Ok(new ResponseModel<List<BookEntity>> { Success = true, Message = "Books retrieved successfully", Data = books });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while fetching all books.");
                return StatusCode(500, new ResponseModel<string> { Success = false, Message = $"Internal server error: {ex.Message}" });
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

                    logger.LogInformation("Books fetched successfully. Count: {Count}");
                    return Ok(new ResponseModel<BookEntity> { Success = true, Message = "Book retrieved successfully", Data = result });
                }
                else
                {
                    logger.LogInformation("Book Not found");
                    return NotFound(new ResponseModel<BookEntity> { Success = false, Message = $"No book found with ID {id}", Data = null });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while fetching book.");
                return StatusCode(500, new ResponseModel<string> { Success = false, Message = $"Internal server error: {ex.Message}" });
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
                    return Unauthorized(new ResponseModel<string>
                    {
                        Success = false,
                        Message = "Unauthorized: Only admins can update books"
                    });
                }

                var result = bookManager.UpdateBookById(id, updatedBook);

                if (result == null)
                {
                    logger.LogInformation("Book Not found");
                    return NotFound(new ResponseModel<BookEntity> { Success = false, Message = $"No book found with ID {id}" });
                }

                logger.LogInformation("Book updated successfully");
                return Ok(new ResponseModel<BookEntity> { Success = true, Message = "Book updated successfully", Data = result });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while fetching book.");
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
                    return Unauthorized(new ResponseModel<string> { Success = false, Message = "Only admins are allowed to add books" });
                }

                var result = bookManager.AddBook(newBook);
                logger.LogInformation("Book added successfully. Title: {Title}", newBook.BookName);

                return Ok(new ResponseModel<BookEntity> { Success = true, Message = "Book added successfully", Data = result });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while adding a new book.");
                return StatusCode(500, new ResponseModel<string> { Success = false, Message = $"Internal server error: {ex.Message}" });
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
                    return Unauthorized(new ResponseModel<string> { Success = false, Message = "Only admins can delete books" });
                }

                var success = bookManager.DeleteBookById(id);

                if (!success)
                {
                    logger.LogWarning("Attempted to delete book, but no book found with ID {BookId}.", id);
                    return NotFound(new ResponseModel<string> { Success = false, Message = $"No book found with ID {id}" });
                }

                logger.LogInformation("Book deleted successfully. Book ID: {BookId}", id);
                return Ok(new ResponseModel<string> { Success = true, Message = "Book deleted successfully" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while deleting book with ID {BookId}.", id);
                return StatusCode(500, new ResponseModel<string> { Success = false, Message = $"Internal server error: {ex.Message}" });
            }
        }


        //[HttpGet("price")]
        //[Authorize]
        //public IActionResult GetBooksSortedByPrice([FromQuery] string order = "asc")
        //{
        //    try
        //    {
        //        var sortedBooks = bookManager.GetBooksSortedByPrice(order);

        //        return Ok(new ResponseModel<List<BookEntity>> { Success = true, Message = $"Books sorted by price ({order})", Data = sortedBooks });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new ResponseModel<string> { Success = false, Message = $"Internal server error: {ex.Message}" });
        //    }
        //}




        [HttpGet("searchbyauthor")]
        [Authorize]
        public IActionResult SearchBooksByAuthor([FromQuery] string author)
        {
            try
            {
                var books = bookManager.SearchBooksByAuthor(author);

                return Ok(new ResponseModel<List<BookEntity>>
                {
                    Success = true,
                    Message = $"Books by author: '{author}'",
                    Data = books
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string> { Success = false, Message = $"Internal server error: {ex.Message}" });
            }
        }


        [HttpGet("sortbookbypriceasc")]
        [Authorize]
        public IActionResult SortBooksByPriceAscending()
        {
            try
            {
                var books = bookManager.SortBooksByPriceAscending();

                return Ok(new ResponseModel<List<BookEntity>>
                {
                    Success = true,
                    Message = "Books sorted by price in ascending order",
                    Data = books
                });
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



        [HttpGet("searchbypricedesc")]
        [Authorize]
        public IActionResult SortBooksByPriceDescending()
        {
            try
            {
                var books = bookManager.SortBooksByPriceDescending();

                return Ok(new ResponseModel<List<BookEntity>>{
                    Success = true,
                    Message = "Books sorted by price in descending order",
                    Data = books});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string>{
                    Success = false,
                    Message = $"Internal server error: {ex.Message}"});
            }
        }



        [HttpGet("search")]
        [Authorize]
        public IActionResult SearchBooksByAuthorOrTitle([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new ResponseModel<string>
                    {
                        Success = false,
                        Message = "Search term cannot be empty."
                    });
                }

                var books = bookManager.SearchBooks(searchTerm);

                return Ok(new ResponseModel<List<BookEntity>>
                {
                    Success = true,
                    Message = $"Books found for: '{searchTerm}'",
                    Data = books
                });
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




    }
}
