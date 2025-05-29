using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Models
{
    public class OrderItemResponseModel
    {
        public string BookName { get; set; }
        public string BookImage { get; set; }
        public string Author { get; set; }

        public int Quantity { get; set; }

        public decimal OriginalPrice { get; set; }      // MRP
        public decimal? DiscountPrice { get; set; }     // Discounted price

        public decimal TotalPrice { get; set; }         // DiscountPrice * Quantity
        public DateTime OrderDate { get; set; }

    }
}
