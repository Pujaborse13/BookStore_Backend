using System;
using System.Collections.Generic;
using System.Text;
using RepositoryLayer.Models;

namespace ManagerLayer.Interface
{
    public interface IWishListManager
    {
        public WishListModel AddToWishList(string token, int bookId);

        public WishListResponseModel GetWishListDetails(string token);

        public string RemoveFromWishlist(string token, int bookId);








    }
}
