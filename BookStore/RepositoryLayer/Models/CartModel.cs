using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Models
{
    public  class CartModel
    {
        public int CustomerId { get; set; }  //userId
        public string UserFullName { get; set; }
        //public string UserLastName { get; set; }
        public string UserEmail { get; set; }
        public int BookId { get; set; }
        public string BookImage { get; set; }
        public string BookTitle { get; set; }
        public string Author { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public bool IsPurchased { get; set; } = false;
    }
}
