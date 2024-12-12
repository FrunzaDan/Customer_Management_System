using CustomerManagementSystem.BusinessLogic.Configuration;
using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Models;

namespace CustomerManagementSystem.BusinessLogic.Services.Implementation;

public class CustomerService : ICustomerService
{
    private readonly IBllConfig _configuration;
    private readonly IDbUtils _dbUtils;

    public CustomerService()
    {
        _configuration = ServiceLocator.GetService<IBllConfig>();
        _dbUtils = ServiceLocator.GetService<IDbUtils>();
    }

    public ResponseModel DeactivateCustomer(string customerGUID)
    {
        throw new NotImplementedException();
    }

    public ResponseModel DeleteCustomer(string customerGUID)
    {
        throw new NotImplementedException();
    }

    public ResponseModel EditCustomerFunction(CustomerModel editCustomerRequest)
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