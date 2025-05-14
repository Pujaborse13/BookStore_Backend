using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
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

        public CartResponseModel GetCartDetails(string token)
        {
            try
            {
                int userId = jwtTokenHelper.ExtractUserIdFromJwt(token);
                string role = jwtTokenHelper.ExtractRoleFromJwt(token);

                if (!string.Equals(role, "user", StringComparison.OrdinalIgnoreCase))
                {
                    return new CartResponseModel
                    {
                        IsSuccess = false,
                        Message = "Only users are allowed to access the cart."
                    };
                }

                var user = context.Users.FirstOrDefault(u => u.UserId == userId);
                if (user == null)
                {
                    return new CartResponseModel{ IsSuccess = false,Message = "User not found."};
                }

                var cartItems = context.Cart
                    .Where(c => c.CustomerId == userId && !c.IsPurchased)
                    .Join(context.Books,
                          cart => cart.BookId,
                          book => book.Id,
                          (cart, book) => new { cart, book })
                    .ToList();

                if (!cartItems.Any())
                {
                    return new CartResponseModel{IsSuccess = false,Message = "Cart is empty or not found."};
                }

                var cartList = cartItems.Select(c => new CartItemModel
                {
                    BookId = c.book.Id,
                    BookName = c.book.BookName,
                    Quantity = c.cart.Quantity,
                    Price = c.cart.Quantity * c.cart.SinglUnitPrice
                }).ToList();

                return new CartResponseModel{
                    IsSuccess = true,
                    Message = "Cart fetched successfully.",
                    Data = new CartSummeryModel
                    {
                        Items = cartList,
                        TotalQuantity = cartList.Sum(x => x.Quantity),
                        TotalCost = cartList.Sum(x => x.Price),
                        User = new UserDetailsModel
                        {
                            UserId = user.UserId,
                            Email = user.Email,
                            FullName = user.FullName
                          
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                return new CartResponseModel
                {
                    IsSuccess = false,
                    Message = $"Internal error: {ex.Message}"
                };
            }
        }


        public CartModel UpdateCartQuantity(string token, int bookId, string action)
        {
            try
            {
                int userId = jwtTokenHelper.ExtractUserIdFromJwt(token);
                string role = jwtTokenHelper.ExtractRoleFromJwt(token);

                if (role.ToLower() != "user")
                    throw new UnauthorizedAccessException("Only users can update the cart.");

                var user = context.Users.FirstOrDefault(u => u.UserId == userId);
                if (user == null)
                    throw new ArgumentException($"User with ID {userId} not found.");

                var cartItem = context.Cart.FirstOrDefault(c =>
                    c.CustomerId == userId && c.BookId == bookId && !c.IsPurchased);

                if (cartItem == null)
                    throw new ArgumentException("Cart item not found.");

                var book = context.Books.FirstOrDefault(b => b.Id == bookId);
                if (book == null)
                    throw new ArgumentException($"Book with ID {bookId} not found.");



                if (action.ToLower() == "+")
                {
                    cartItem.Quantity += 1;
                }
                else if (action.ToLower() == "-")
                {
                    cartItem.Quantity -= 1;
                    if (cartItem.Quantity <= 0)
                    {
                        context.Cart.Remove(cartItem);
                        context.SaveChanges();
                        return null; // Item removed
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid action. Use '-' or '+'.");
                }

                cartItem.SinglUnitPrice = (decimal)book.Price;
                context.SaveChanges();

                return new CartModel
                {
                    CustomerId = userId,
                    UserFullName = user.FullName,
                    UserEmail = user.Email,
                    BookId = bookId,
                    BookTitle = book.BookName,
                    Quantity = cartItem.Quantity,
                    Price = (decimal)(book.Price * cartItem.Quantity),
                    IsPurchased = false
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating cart quantity: {ex.Message}");
                return null;
            }
        }




    }
}
    







