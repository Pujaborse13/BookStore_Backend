using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Helper;
using RepositoryLayer.Interface;
using StackExchange.Redis;


namespace RepositoryLayer.Service
{
   public class BookRepo : IBookRepo
   {
        private readonly BookStoreDBContext context;
        private readonly IConfiguration configuration;
        private readonly JwtTokenHelper jwtTokenHelper;
        private readonly IConnectionMultiplexer redis;
        private readonly StackExchange.Redis.IDatabase redisDb;

        public BookRepo(BookStoreDBContext context, IConfiguration configuration, JwtTokenHelper jwtTokenHelper, IConnectionMultiplexer redis)
        {
            this.context = context;
            this.configuration = configuration;
            this.jwtTokenHelper = jwtTokenHelper;
            this.redis = redis;
            this.redisDb = redis.GetDatabase();
        }


        public string LoadBooksFromCsv(string token)
        {
            var role = jwtTokenHelper.ExtractRoleFromJwt(token);
            if (!string.Equals(role?.Trim(), "admin", StringComparison.OrdinalIgnoreCase))
            {
                return "Unauthorized: Only admin can load books.";
            }


            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string relativePath = @"..\..\..\..\RepositoryLayer\Helper\books.csv";

                string fullPath = Path.GetFullPath(Path.Combine(basePath, relativePath));

                var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HeaderValidated = null,
                    MissingFieldFound = null
                };

                using (var reader = new StreamReader(fullPath))
                using (var csv = new CsvReader(reader, config))
                {
                    var records = csv.GetRecords<BookEntity>().ToList();

                    foreach (var book in records)
                    {
                        book.Id = 0;
                        book.CreatedAt = DateTime.Now;
                        book.UpdatedAt = DateTime.Now;
                    }

                    context.Books.AddRange(records);
                    context.SaveChanges();
                    redisDb.KeyDelete("bookstore:allbooks"); // Invalidate cache

                    return "Books loaded successfully from CSV.";
                }
            }

            catch (DbUpdateException dbEx)
            {
                return $"EF Core Error: {dbEx.InnerException?.Message ?? dbEx.Message}";
            }
            catch (Exception ex)
            {
                return $"General Error: {ex.Message}";
            }
            
        }
        /*
        public List<BookEntity> GetAllBooks()
        {
             try
             {
                 return context.Books.ToList();
             }
             catch (Exception ex)
             {
                 return new List<BookEntity>();
             }
        }
        */

        // //using radis
        public List<BookEntity> GetAllBooks()
        {
            //using radis
            const string cacheKey = "bookstore:allbooks";  // key store list of books in Redis.
            string cachedBooks = redisDb.StringGet(cacheKey); //get the cached value using the cacheKey


            if (!string.IsNullOrEmpty(cachedBooks))
            {
                Console.WriteLine("Data retrieved from Redis cache.");

                return JsonSerializer.Deserialize<List<BookEntity>>(cachedBooks);
            }

            // If not in cache, fetch from DB
            Console.WriteLine("Data not in Redis. Fetching from database.");
            var books = context.Books.ToList();   // If not in cache, fetch from DB

            // Cache the result with 30 mins expiry
            var serializedBooks = JsonSerializer.Serialize(books);
            redisDb.StringSet(cacheKey, serializedBooks, TimeSpan.FromMinutes(30));

            return books;

        }


        public BookEntity GetBookById(int id)
        {
            return context.Books.FirstOrDefault(b => b.Id == id);
        }

        public BookEntity UpdateBookById(int bookId, BookEntity updatedBook)
        {
            var book = context.Books.FirstOrDefault(b => b.Id == bookId);
            if (book == null) 
                return null;

            book.BookName = updatedBook.BookName;
            book.Author = updatedBook.Author;
            book.Description = updatedBook.Description;
            book.Price = updatedBook.Price;
            book.DiscountPrice = updatedBook.DiscountPrice;
            book.Quantity = updatedBook.Quantity;
            book.BookImage = updatedBook.BookImage;
            book.UpdatedAt = DateTime.Now;

            context.SaveChanges();
            redisDb.KeyDelete("bookstore:allbooks"); // Invalidate cache
            return book;
        }


        public BookEntity AddBook(BookEntity newBook)
        {
            newBook.CreatedAt = DateTime.Now;
            newBook.UpdatedAt = DateTime.Now;

            context.Books.Add(newBook);
            context.SaveChanges();
            redisDb.KeyDelete("bookstore:allbooks"); // Invalidate cache
            return newBook;
        }


        public bool DeleteBookById(int id)
        {
            var book = context.Books.FirstOrDefault(b => b.Id == id);
            if (book == null) 
                return false;

            context.Books.Remove(book);
            context.SaveChanges();
            redisDb.KeyDelete("bookstore:allbooks"); // Invalidate cache
            return true;
        }



        public List<BookEntity> GetBooksSortedByPrice(string order)
        {
            return order.ToLower() == "desc"
                ? context.Books.OrderByDescending(b => b.Price).ToList()
                : context.Books.OrderBy(b => b.Price).ToList();
        }


        public List<BookEntity> SearchBooksByAuthor(string authorName)
        {
            return context.Books
                          .Where(b => b.Author != null && b.Author.ToLower().Contains(authorName.ToLower()))
                          .ToList();
        }

        public List<BookEntity> SortBooksByPriceAscending()
        {
            return context.Books.OrderBy(b => b.DiscountPrice).ToList();
        }

        public List<BookEntity> SortBooksByPriceDescending()
        {
            return context.Books.OrderByDescending(b => b.DiscountPrice).ToList();
        }



        public List<BookEntity> SearchBooks(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<BookEntity>();

            searchTerm = searchTerm.ToLower();

            return context.Books
                .Where(b =>
                    (b.Author != null && b.Author.ToLower().Contains(searchTerm)) ||
                    (b.BookName != null && b.BookName.ToLower().Contains(searchTerm))
                )
                .ToList();
        }





   }
}
