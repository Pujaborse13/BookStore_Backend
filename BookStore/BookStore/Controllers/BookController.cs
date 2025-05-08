using ManagerLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Context;
using RepositoryLayer.Helper;

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
            var books = bookManager.GetAllBooks();
            return Ok(books);
        }



        


    }
}
