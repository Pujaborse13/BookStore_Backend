using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Models
{
    public class WishListResponseModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public WishListSummeryModel Data { get; set; }
    }
}
