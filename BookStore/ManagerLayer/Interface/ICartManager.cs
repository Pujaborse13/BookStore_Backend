using System;
using System.Collections.Generic;
using System.Text;
using RepositoryLayer.Models;

namespace ManagerLayer.Interface
{
    public interface ICartManager
    {
        public CartModel AddToCart(string token, int bookId);
        public CartResponseModel GetCartDetails(string token);


    }
}
