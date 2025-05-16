using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Models
{
    public class WishListModel
    {
        public int AddedBy { get; set; }  // UserId
        public string UserFullName{ get; set; }
        public string UserEmail { get; set; }

        public int BookId { get; set; }
        public string BookName { get; set; }

        public string Author { get; set; }
        public string? Description { get; set; }

        public decimal? Price { get; set; }

        public decimal? DiscountPrice { get; set; }
        public int? Quantity { get; set; }

        public string BookImage { get; set; }

    }
}
