using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Helper;
using RepositoryLayer.Interface;
using RepositoryLayer.Models;

namespace RepositoryLayer.Service
{
    public class CartRepo : ICartRepo
    {
        private readonly BookStoreDBContext context;
        private readonly JwtTokenHelper jwtTokenHelper;
        public CartRepo(BookStoreDBContext context, JwtTokenHelper jwtTokenHelper)
        {
            this.context = context;
            this.jwtTokenHelper = jwtTokenHelper;

        }

        public CartModel AddToCart(string token, int bookId)
        {

            if (string.IsNullOrWhiteSpace(token))
                throw new UnauthorizedAccessException("Authorization token is missing.");

            var role = jwtTokenHelper.ExtractRoleFromJwt(token);
            int userId = jwtTokenHelper.ExtractUserIdFromJwt(token);

                if (role.ToLower() != "user")
                    throw new UnauthorizedAccessException("Only users can add to cart. Admins are not allowed.");
               
                var book = context.Books.FirstOrDefault(b => b.Id == bookId);
                if (book == null)
                    throw new ArgumentException($"Book with ID {bookId} not found.");

                var user = context.Users.FirstOrDefault(u => u.UserId == userId);
                if (user == null)
                    throw new ArgumentException($"User with ID {userId} not found.");
                    

                var existingCartItem = context.Cart
                    .FirstOrDefault(c => c.CustomerId == userId && c.BookId == bookId && !c.IsPurchased);

                if (existingCartItem != null)
                {
                    existingCartItem.Quantity += 1;
                    existingCartItem.SinglUnitPrice = (decimal)book.Price;
                    context.Cart.Update(existingCartItem);
                }
                else
                {
                    var newCart = new CartEntity
                    {
                        CustomerId = userId,
                        BookId = bookId,
                        Quantity = 1,
                        SinglUnitPrice = (decimal)book.Price,
                        IsPurchased = false
                    };
                    context.Cart.Add(newCart);
                    existingCartItem = newCart;
                }

                context.SaveChanges();

                return new CartModel
                {
                    CustomerId = userId,
                    BookId = bookId,
                    Quantity = existingCartItem != null ? existingCartItem.Quantity : 1,
                    Price = existingCartItem != null
                        ? existingCartItem.SinglUnitPrice * existingCartItem.Quantity
                        : (decimal)book.Price,
                    IsPurchased = false,
                    UserFullName = user.FullName,
                    UserEmail = user.Email,
                    BookTitle = book.BookName

                };
            }
            
        }
    }







