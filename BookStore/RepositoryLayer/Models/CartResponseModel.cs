﻿using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Models
{
    public class CartResponseModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public CartSummeryModel Data { get; set; }
    }
}
