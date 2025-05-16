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
    public class OrderDetailsRepo : IOrderDetailsRepo
    {

        private readonly BookStoreDBContext context;
        private readonly JwtTokenHelper jwtTokenHelper;
        public OrderDetailsRepo(BookStoreDBContext context, JwtTokenHelper jwtTokenHelper)
        {
            this.context = context;
            this.jwtTokenHelper = jwtTokenHelper;
        }

        public OrderModel PlaceOrder(string token)
        {
            var userId = jwtTokenHelper.ExtractUserIdFromJwt(token);
            var role = jwtTokenHelper.ExtractRoleFromJwt(token);

            if (!string.Equals(role, "user", StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException("Only users can place orders.");

            // Fetch cart items
            var cartItems = context.Cart
                .Include(c => c.BookEntity)
                .Include(c => c.UserEntity)
                .Where(c => c.CustomerId == userId && !c.IsPurchased)
                .ToList();

            if (cartItems == null || cartItems.Count == 0)
                throw new InvalidOperationException("No items in the cart to place an order.");

            var orderResponses = new List<OrderResponseModel>();

            foreach (var item in cartItems)
            {
                var book = item.BookEntity;

                // Check stock availability
                if (book.Quantity < item.Quantity)
                {
                    throw new InvalidOperationException(
                        $"Not enough quantity for book '{book.BookName}'. " +
                        $"Available: {book.Quantity}, Requested: {item.Quantity}");
                }

                // Reduce book stock
                book.Quantity -= item.Quantity;

                var totalPrice = item.SinglUnitPrice * item.Quantity;

                var order = new OrderDetailsEntity
                {
                    UserId = userId,
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    TotalPrice = totalPrice,
                    OrderDate = DateTime.UtcNow
                };

                // Add order to DB
                context.OrderDetails.Add(order);
                context.SaveChanges(); 

                //return after order place
                var response = new OrderResponseModel
                {
                    OrderId = order.OrderId,
                    UserId = userId,
                    UserFullName = item.UserEntity.FullName,
                    UserEmail = item.UserEntity.Email,
                    BookId = item.BookId,
                    BookName = book.BookName,
                    Author = book.Author,
                    Price = totalPrice,
                    Quantity = item.Quantity,
                    OrderDate = order.OrderDate
                };

                orderResponses.Add(response);
            }

            // Remove purchased items from cart
            context.Cart.RemoveRange(cartItems);
            context.SaveChanges();

            return new OrderModel
            {
                Message = "Order placed successfully.",
                Orders = orderResponses
            };
        }



    }
}
