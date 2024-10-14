using CustomerManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace CustomerManagementSystem.BusinessLogic.Services
{
    public interface ICustomerService
    {
        public CustomerListModel GetCustomers();

        public CustomerModel GetCustomer(GetCustomerRequest getCustomerRqst);

        public ResponseModel RegisterCustomerFunction(CustomerModel customerRqst);

        public ResponseModel EditCustomerFunction(CustomerModel editCustomerRqst);

        public ResponseModel DeactivateCustomer(string customerGUID);

        public ResponseModel DeleteCustomer(string customerGUID);

    }
}