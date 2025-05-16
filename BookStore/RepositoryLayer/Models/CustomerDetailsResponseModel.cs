using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Models
{
    public class CustomerDetailsResponseModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Type { get; set; }
    }
}
