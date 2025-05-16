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
            // Extract user info from token
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
                var totalPrice = item.SinglUnitPrice * item.Quantity;

                var order = new OrderDetailsEntity
                {
                    UserId = userId,
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    TotalPrice = totalPrice,
                    OrderDate = DateTime.UtcNow
                };

                context.OrderDetails.Add(order);
                context.SaveChanges(); // Save to get OrderId populated

                var response = new OrderResponseModel
                {
                    OrderId = order.OrderId,
                    OrderedBy = userId,
                    UserFullName = item.UserEntity.FullName,
                    UserEmail = item.UserEntity.Email,
                    BookId = item.BookId,
                    BookName = item.BookEntity.BookName,
                    Author = item.BookEntity.Author,
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
