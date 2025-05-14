using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Models
{
    public class UserDetailsModel
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
    }
}
