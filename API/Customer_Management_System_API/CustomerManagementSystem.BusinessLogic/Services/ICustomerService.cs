using CustomerManagementSystem.Domain.Models;

namespace CustomerManagementSystem.BusinessLogic.Services;

public interface ICustomerService
{
    Task<ResponseModel<CustomerListModel>> GetCustomers();

    Task<ResponseModel<CustomerModel>> GetCustomer(GetCustomerRequest getCustomerRequest);

    Task<ResponseModel<object>> RegisterCustomer(CustomerModel customerRequest);

    Task<ResponseModel<object>> EditCustomer(CustomerModel editCustomerRequest);

    Task<ResponseModel<object>> DeactivateCustomer(string customerGuid);

    Task<ResponseModel<object>> DeleteCustomer(string customerGuid);
}