using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Models
{
    public class WishlListItemModel
    {
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
