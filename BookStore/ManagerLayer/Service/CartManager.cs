using System;
using System.Collections.Generic;
using System.Text;
using ManagerLayer.Interface;
using Microsoft.Extensions.Configuration;
using RepositoryLayer.Interface;
using RepositoryLayer.Models;
using RepositoryLayer.Service;

namespace ManagerLayer.Service
{
    public class CartManager : ICartManager
    {
        private readonly ICartRepo cartRepo;
        private readonly IConfiguration configuration;

        public CartManager(ICartRepo cartRepo, IConfiguration configuration)
        {
            this.cartRepo = cartRepo;
            this.configuration = configuration;
        }


        public CartModel AddToCart(string token, int bookId)
        { 
                 return cartRepo.AddToCart(token, bookId);
        }

        public CartResponseModel GetCartDetails(string token)
        {
            return  cartRepo.GetCartDetails(token);
        }

        public CartModel UpdateCartQuantity(string token, int bookId, string action)
        {
            return cartRepo.UpdateCartQuantity(token, bookId, action);
        }


        public string DeleteFromCartIfQuantityZero(string token, int bookId)
        { 
            return cartRepo.DeleteFromCartIfQuantityZero(token, bookId);
        }



    }
}
