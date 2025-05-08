using System;
using System.Collections.Generic;
using System.Text;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface IBookRepo
    {
        public string LoadBooksFromCsv(string token);
        public List<BookEntity> GetAllBooks();
        public BookEntity GetBookById(int id);
        public BookEntity UpdateBookById(int bookId, BookEntity updatedBook);
        public BookEntity AddBook(BookEntity newBook);
        public bool DeleteBookById(int id);
        public List<BookEntity> GetBooksSortedByPrice(string order);










    }
}
