using System;
using System.Collections.Generic;
using System.Text;
using RepositoryLayer.Models;

namespace RepositoryLayer.Interface
{
    public interface ICartRepo
    {
        public CartModel AddToCart(string token, int bookId);

    }
}
