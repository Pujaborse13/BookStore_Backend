using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Models
{
    public class OrderModel
    {
        public string Message { get; set; }
        public List<OrderResponseModel> Orders { get; set; }
    }
}
