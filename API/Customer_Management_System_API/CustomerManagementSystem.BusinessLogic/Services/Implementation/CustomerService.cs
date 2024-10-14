using CustomerManagementSystem.BusinessLogic.CustomerFunctions;
using CustomerManagementSystem.BusinessLogic.Configuration;
using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace CustomerManagementSystem.BusinessLogic.Services.Implementation;

public class CustomerService : ICustomerService
{
    private readonly IBLLConfig _configuration;
    private readonly IDBUtils _dbUtils;

    public CustomerService()
    {
        _configuration = ServiceLocator.GetService<IBLLConfig>();
        _dbUtils = ServiceLocator.GetService<IDBUtils>();
    }

    public ResponseModel DeactivateCustomer(string customerGUID)
    {
        throw new NotImplementedException();
    }

    public ResponseModel DeleteCustomer(string customerGUID)
    {
        throw new NotImplementedException();
    }

    public ResponseModel EditCustomerFunction(CustomerModel editCustomerRqst)
    {
        throw new NotImplementedException();
    }

    public CustomerModel GetCustomer(GetCustomerRequest getCustomerRqst)
    {
        throw new NotImplementedException();
    }

    public CustomerListModel GetCustomers()
    {
        throw new NotImplementedException();
    }

    public ResponseModel RegisterCustomerFunction(CustomerModel customerRqst)
    {
        throw new NotImplementedException();
    }
}
