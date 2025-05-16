using System;
using System.Collections.Generic;
using System.Text;
using ManagerLayer.Interface;
using RepositoryLayer.Interface;
using RepositoryLayer.Models;

namespace ManagerLayer.Service
{
    public class CustomerDetailsManager : ICustomerDetailsManager
    {
        private readonly ICustomerDetailsRepo customerDetailsRepo;

        public CustomerDetailsManager(ICustomerDetailsRepo customerDetailsRepo)
        {
            this.customerDetailsRepo = customerDetailsRepo;
        }

        public CustomerDetailsModel AddCustomerDetails(CustomerDetailsModel model, string token)

        {
            return customerDetailsRepo.AddCustomerDetails(model, token);
        }
    }
}
