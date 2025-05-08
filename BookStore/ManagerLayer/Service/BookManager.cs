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


       
    }
}
