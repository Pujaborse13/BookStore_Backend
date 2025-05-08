using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Context
{
    public class BookStoreDBContext : DbContext
    {
       public BookStoreDBContext(DbContextOptions option) : base(option) { }
       public DbSet<UserEntity> Users { get; set; }
       public DbSet<AdminEntity> Admins { get; set; }
        public DbSet<BookEntity> Books { get; set; }


    }
}
