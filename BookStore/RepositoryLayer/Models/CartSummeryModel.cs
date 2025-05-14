using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Models
{
    public class CartSummeryModel
    {
        public UserDetailsModel User { get; set; }
        public List<CartItemModel> Items { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalCost { get; set; }

    }
}
