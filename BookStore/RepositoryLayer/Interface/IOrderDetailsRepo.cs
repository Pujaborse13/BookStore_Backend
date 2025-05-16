using System;
using System.Collections.Generic;
using System.Text;
using RepositoryLayer.Models;

namespace RepositoryLayer.Interface
{
    public interface IOrderDetailsRepo
    {
        public OrderModel PlaceOrder(string token);

    }
}
