using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Helper;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
   public class BookRepo : IBookRepo
   {
        private readonly BookStoreDBContext context;
        private readonly IConfiguration configuration;
        private readonly JwtTokenHelper jwtTokenHelper;

        public BookRepo(BookStoreDBContext context, IConfiguration configuration, JwtTokenHelper jwtTokenHelper)
        {
            this.context = context;
            this.configuration = configuration;
            this.jwtTokenHelper = jwtTokenHelper;

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
            return book;
        }


        public BookEntity AddBook(BookEntity newBook)
        {
            newBook.CreatedAt = DateTime.Now;
            newBook.UpdatedAt = DateTime.Now;

            context.Books.Add(newBook);
            context.SaveChanges();

            return newBook;
        }


        public bool DeleteBookById(int id)
        {
            var book = context.Books.FirstOrDefault(b => b.Id == id);
            if (book == null) 
                return false;

            context.Books.Remove(book);
            context.SaveChanges();
            return true;
        }

    }
}
