using System;
using System.Collections.Generic;
using System.Text;
using RepositoryLayer.Models;

namespace ManagerLayer.Interface
{
    public interface IOrderDetailsManager
    {
        public OrderModel PlaceOrder(string token);
        public List<OrderItemResponseModel> GetOrdersByUser(string token);


    }
}
