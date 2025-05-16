using System;
using System.Collections.Generic;
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
    public class CustomerDetailsRepo :ICustomerDetailsRepo
    {
        private readonly BookStoreDBContext context;
        private readonly JwtTokenHelper jwtTokenHelper;
        public CustomerDetailsRepo(BookStoreDBContext context, JwtTokenHelper jwtTokenHelper)
        {
            this.context = context;
            this.jwtTokenHelper = jwtTokenHelper;
        }

        public CustomerDetailsModel AddCustomerDetails(CustomerDetailsModel model, string token)
        {
            int userId = jwtTokenHelper.ExtractUserIdFromJwt(token);
            string role = jwtTokenHelper.ExtractRoleFromJwt(token);

            if (role != "user")
            {
                throw new UnauthorizedAccessException("Only Users can add customer details.");
            }

            var user = context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            var customer = new CustomerDetailsEntity
            {
                FullName = model.FullName,
                Mobile = model.Mobile,
                Address = model.Address,
                City = model.City,
                State = model.State,
                Type = model.Type,
                UserId = userId
            };

            context.CustomerDetails.Add(customer);
            context.SaveChanges();

            // Map entity to model to return
            return new CustomerDetailsModel
            {
                FullName = customer.FullName,
                Mobile = customer.Mobile,
                Address = customer.Address,
                City = customer.City,
                State = customer.State,
                Type = customer.Type
            };
        }
    }



}
    
