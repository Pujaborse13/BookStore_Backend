using System;
using System.Collections.Generic;
using System.Text;
using RepositoryLayer.Models;

namespace RepositoryLayer.Interface
{
    public interface ICustomerDetailsRepo
    {
        public CustomerDetailsModel AddCustomerDetails(CustomerDetailsModel model, string token);
        public List<CustomerDetailsResponseModel> GetAllCustomerDetails(string token);


    }
}
