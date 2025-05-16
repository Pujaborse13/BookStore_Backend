using System;
using System.Collections.Generic;
using System.Text;
using RepositoryLayer.Models;

namespace ManagerLayer.Interface
{
    public interface ICustomerDetailsManager
    {
        public CustomerDetailsModel AddCustomerDetails(CustomerDetailsModel model, string token);
        public List<CustomerDetailsResponseModel> GetAllCustomerDetails(string token);



    }
}
