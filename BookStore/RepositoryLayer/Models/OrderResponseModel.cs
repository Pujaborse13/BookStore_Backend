using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Models
{
    public class OrderResponseModel
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string UserFullName { get; set; }
        public string UserEmail { get; set; }
        public int BookId { get; set; }
        public string BookName { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public DateTime OrderDate { get; set; }
        public decimal PricePerItem { get; set; }
        public decimal? DiscountPrice { get; set; }
    }
}
