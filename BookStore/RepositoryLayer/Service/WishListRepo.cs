using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Helper;
using RepositoryLayer.Interface;
using RepositoryLayer.Models;

namespace RepositoryLayer.Service
{
    public class WishListRepo : IWishListRepo
    {

        private readonly BookStoreDBContext context;
        private readonly JwtTokenHelper jwtTokenHelper;
        public WishListRepo(BookStoreDBContext context, JwtTokenHelper jwtTokenHelper)
        {
            this.context = context;
            this.jwtTokenHelper = jwtTokenHelper;

        }

        public WishListModel AddToWishList(string token, int bookId)
        {
            if (string.IsNullOrEmpty(token))
                throw new UnauthorizedAccessException("Authorization token is missing.");

            var role = jwtTokenHelper.ExtractRoleFromJwt(token);
            var userId = jwtTokenHelper.ExtractUserIdFromJwt(token);

            if (role.ToLower() != "user")
                throw new UnauthorizedAccessException("Only users can add to wishlist.");

            var book = context.Books.FirstOrDefault(b => b.Id == bookId);
            if (book == null)
                throw new ArgumentException($"Book with ID {bookId} not found.");

            var user = context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
                throw new ArgumentException($"User with ID {userId} not found.");

            var existingItem = context.WishList.FirstOrDefault(c => c.AddedBy == userId && c.BookId == bookId);
            if (existingItem != null)
                throw new InvalidOperationException("Book already exists in wishlist.");

            var newWishlist = new WishListEntity
            {
                AddedBy = userId,
                BookId = bookId,
            };
            context.WishList.Add(newWishlist);
            context.SaveChanges();

            return new WishListModel
            {
                AddedBy = userId,
                BookId = bookId,
                BookName = book.BookName,
                Author = book.Author,
                Description = book.Description,
                Price = book.Price,
                DiscountPrice = book.DiscountPrice,
                Quantity = book.Quantity,
                BookImage = book.BookImage,
                UserFullName = user.FullName,
                UserEmail = user.Email
            };
        }




    }
}
