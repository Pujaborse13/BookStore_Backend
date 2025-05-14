using System;
using System.Collections.Generic;
using System.Text;
using RepositoryLayer.Models;

namespace RepositoryLayer.Interface
{
    public interface ICartRepo
    {
        public CartModel AddToCart(string token, int bookId);
        public CartResponseModel GetCartDetails(string token);
        public CartModel UpdateCartQuantity(string token, int bookId, string action);
        public string DeleteFromCartIfQuantityZero(string token, int bookId);





    }
}
