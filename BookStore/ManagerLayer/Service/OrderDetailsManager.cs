using System;
using System.Collections.Generic;
using System.Text;
using ManagerLayer.Interface;
using RepositoryLayer.Interface;
using RepositoryLayer.Models;

namespace ManagerLayer.Service
{
    public class OrderDetailsManager:IOrderDetailsManager
    {
        private readonly IOrderDetailsRepo orderDetailsRepo;

        public OrderDetailsManager(IOrderDetailsRepo orderDetailsRepo)
        {
           this.orderDetailsRepo = orderDetailsRepo;
        }
        public OrderModel PlaceOrder(string token)
        { 
           return orderDetailsRepo.PlaceOrder(token);
        }

    }
}
