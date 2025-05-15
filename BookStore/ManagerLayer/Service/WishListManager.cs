using System;
using System.Collections.Generic;
using System.Text;
using ManagerLayer.Interface;
using Microsoft.Extensions.Configuration;
using RepositoryLayer.Interface;
using RepositoryLayer.Models;

namespace ManagerLayer.Service
{
    public class WishListManager : IWishListManager
    {
        private readonly IWishListRepo wishListRepo;
        private readonly IConfiguration configuration;

        public WishListManager(IWishListRepo wishListRepo , IConfiguration configuration)
        {
            this.wishListRepo = wishListRepo;
            this.configuration = configuration;
        }

        public WishListModel AddToWishList(string token, int bookId)
        {
            return wishListRepo.AddToWishList(token, bookId);
        }

        public WishListResponseModel GetWishListDetails(string token)
        { 
            return wishListRepo.GetWishListDetails(token);
        }


        public string RemoveFromWishlist(string token, int bookId)
        {
            return wishListRepo.RemoveFromWishlist(token, bookId);
        }


    }
}
