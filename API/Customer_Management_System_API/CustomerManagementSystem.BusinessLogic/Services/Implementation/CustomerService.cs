using CustomerManagementSystem.BusinessLogic.Configuration;
using CustomerManagementSystem.BusinessLogic.CustomerFunctions;
using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Models;

namespace CustomerManagementSystem.BusinessLogic.Services.Implementation;

public class CustomerService : ICustomerService
{
    private readonly IBllConfig _configuration = ServiceLocator.GetService<IBllConfig>();
    private readonly IDbUtils _dbUtils = ServiceLocator.GetService<IDbUtils>();

    public async Task<ResponseModel> DeactivateCustomer(string customerGuid)
    {
        var customerDeactivation = new CustomerDeactivation(_dbUtils);
        var response = await customerDeactivation.DeactivateCustomer(customerGuid);
        return response;
    }

    public async Task<ResponseModel> DeleteCustomer(string customerGuid)
    {
        var customerDeletion = new CustomerDeletion(_dbUtils);
        var response = await customerDeletion.DeleteCustomer(customerGuid);
        return response;
    }

    public async Task<ResponseModel> EditCustomer(CustomerModel editCustomerRequest)
    {
        var customerEditing = new CustomerEditing(_dbUtils);
        var response = await customerEditing.EditCustomerFunction(editCustomerRequest);
        return response;
    }

    public async Task<CustomerModel> GetCustomer(GetCustomerRequest getCustomerRqst)
    {
        var customerGetting = new CustomerGetting(_dbUtils);
        var response = await customerGetting.GetCustomerFunction(getCustomerRqst);
        return response;
    }

    public async Task<CustomerListModel> GetCustomers()
    {
        var customerGetting = new CustomerGetting(_dbUtils);
        var response = await customerGetting.GetCustomersFunction();
        return response;
    }

    public async Task<ResponseModel> RegisterCustomer(CustomerModel customerRqst)
    {
        var customerRegistration = new CustomerRegistration(_dbUtils);
        var response = await customerRegistration.RegisterCustomerFunction(customerRqst);
        return response;
    }
}