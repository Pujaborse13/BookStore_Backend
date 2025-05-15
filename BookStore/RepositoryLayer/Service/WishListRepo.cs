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

        public WishListResponseModel GetWishListDetails(string token)
        {
            try
            {
                int userId = jwtTokenHelper.ExtractUserIdFromJwt(token);
                string role = jwtTokenHelper.ExtractRoleFromJwt(token);

                if (role.ToLower() != "user")
                    return new WishListResponseModel { IsSuccess = false, Message = "Only users are allowed to access the wishlist." };

                var user = context.Users.FirstOrDefault(u => u.UserId == userId);
                if (user == null)
                    return new WishListResponseModel { IsSuccess = false, Message = "User not found." };

                var wishlistItems = context.WishList
                    .Where(c => c.AddedBy == userId)
                    .Include(c => c.BookEntity)
                    .ToList();

                if (!wishlistItems.Any())
                    return new WishListResponseModel { IsSuccess = false, Message = "wishlist is empty or not found." };

                var wishlistList = wishlistItems.Select(c => new WishlListItemModel
                {
                    BookId = c.BookId,
                    BookName = c.BookEntity.BookName,
                    Author = c.BookEntity.Author,
                    Description = c.BookEntity.Description,
                    Price = c.BookEntity.Price,
                    DiscountPrice = c.BookEntity.DiscountPrice,
                    //Quantity = c.BookEntity.Quantity,
                    BookImage = c.BookEntity.BookImage


                }).ToList();

                return new WishListResponseModel
                {
                    IsSuccess = true,
                    Message = "wishlist fetched successfully.",
                    Data = new WishListSummeryModel
                    {
                        Items = wishlistList,

                        User = new UserDetailsModel
                        {
                            UserId = user.UserId,
                            Email = user.Email,
                            FullName = user.FullName,
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                return new WishListResponseModel { IsSuccess = false, Message = $"Internal error: {ex.Message}" };
            }
        }

        public string RemoveFromWishlist(string token, int bookId)
        {
            try
            {
                var role = jwtTokenHelper.ExtractRoleFromJwt(token);
                var userId = jwtTokenHelper.ExtractUserIdFromJwt(token);

                if (string.IsNullOrEmpty(role) || role.ToLower() != "user")
                    return "Unauthorized. Only users can delete from wishlist.";

                var wishlistItem = context.WishList.FirstOrDefault(c => c.BookId == bookId && c.AddedBy == userId);

                if (wishlistItem == null)
                    return "wishlist item not found.";


                context.WishList.Remove(wishlistItem);
                context.SaveChanges();

                return "wishlist item deleted successfully.";
            }

            catch (Exception ex)
            {

                return $"An error occurred while deleting the wishlist item: {ex.Message}";
            }



        }

    }
}


