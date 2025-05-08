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








    }
}
