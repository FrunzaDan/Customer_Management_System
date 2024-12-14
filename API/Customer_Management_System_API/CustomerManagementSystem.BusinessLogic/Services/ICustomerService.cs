using CustomerManagementSystem.Domain.Models;

namespace CustomerManagementSystem.BusinessLogic.Services;

public interface ICustomerService
{
    public Task<CustomerListModel> GetCustomers();

    public Task<CustomerModel> GetCustomer(GetCustomerRequest getCustomerRequest);

    public Task<ResponseModel> RegisterCustomer(CustomerModel customerRequest);

    public Task<ResponseModel> EditCustomer(CustomerModel editCustomerRequest);

    public Task<ResponseModel> DeactivateCustomer(string customerGuid);

    public Task<ResponseModel> DeleteCustomer(string customerGuid);
}