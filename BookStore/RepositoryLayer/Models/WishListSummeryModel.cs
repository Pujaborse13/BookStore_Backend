using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Models
{
    public class WishListSummeryModel
    {

        public UserDetailsModel User { get; set; }
        public List<WishlListItemModel> Items { get; set; }
    }
}
