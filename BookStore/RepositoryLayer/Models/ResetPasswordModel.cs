using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Models
{
    public class ResetPasswordModel
    {
        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }
}
