using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Models
{
    public class CartItemModel
    {
        public int BookId { get; set; }
        public string BookName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
