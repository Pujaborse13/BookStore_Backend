using System;
using System.Collections.Generic;
using System.Text;
using ManagerLayer.Interface;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace ManagerLayer.Service
{
    public class BookManager : IBookManager
    {
        private readonly IBookRepo bookRepo;

        public BookManager(IBookRepo bookRepo)
        {
            this.bookRepo = bookRepo;
        }

        public string LoadBooksFromCsv(string token)
        {
            return bookRepo.LoadBooksFromCsv(token);
        }

        public List<BookEntity> GetAllBooks()
        {
            return bookRepo.GetAllBooks();
        }



        public BookEntity GetBookById(int id)
        {
            return bookRepo.GetBookById(id);
        }

        public BookEntity UpdateBookById(int bookId, BookEntity updatedBook)
        {
            return bookRepo.UpdateBookById(bookId, updatedBook);
        }


        public BookEntity AddBook(BookEntity newBook)
        {
            return bookRepo.AddBook(newBook);
        }


        public bool DeleteBookById(int id)
        {
            return bookRepo.DeleteBookById(id);
        }

        public List<BookEntity> GetBooksSortedByPrice(string order)
        {
            return bookRepo.GetBooksSortedByPrice(order);
        }

        public List<BookEntity> SearchBooksByAuthor(string authorName)
        {
            return bookRepo.SearchBooksByAuthor(authorName);
        }


        public List<BookEntity> SortBooksByPriceAscending()
        {
            return bookRepo.SortBooksByPriceAscending();
        }


        public List<BookEntity> SortBooksByPriceDescending()
        {
            return bookRepo.SortBooksByPriceDescending();
        }

        public List<BookEntity> SearchBooks(string searchTerm)
        {
            return bookRepo.SearchBooks(searchTerm);
        }

    }
}
