using System;
using System.Collections.Generic;
using System.Text;
using RepositoryLayer.Models;

namespace RepositoryLayer.Interface
{
    public interface IWishListRepo
    {
        public WishListModel AddToWishList(string token, int bookId);
        public WishListResponseModel GetWishListDetails(string token);



    }
}
