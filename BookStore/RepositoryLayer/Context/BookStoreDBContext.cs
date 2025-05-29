using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Entity;
using RepositoryLayer.Models;

namespace RepositoryLayer.Context
{
    public class BookStoreDBContext : DbContext
    {
       public BookStoreDBContext(DbContextOptions option) : base(option) { }
       public DbSet<UserEntity> Users { get; set; }
       public DbSet<AdminEntity> Admins { get; set; }
        public DbSet<BookEntity> Books { get; set; }
        public DbSet<CartEntity> Cart { get; set; }
        public DbSet<WishListEntity> WishList { get; set; }
        public DbSet<CustomerDetailsEntity> CustomerDetails { get; set; }
        public DbSet<OrderDetailsEntity> OrderDetails{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WishlListItemModel>().HasNoKey(); // <-- Add this line

            base.OnModelCreating(modelBuilder);
        }







    }
}
