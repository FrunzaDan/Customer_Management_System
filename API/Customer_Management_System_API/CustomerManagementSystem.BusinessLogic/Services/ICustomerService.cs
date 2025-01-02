using CustomerManagementSystem.Domain.Models;

namespace CustomerManagementSystem.BusinessLogic.Services;

public interface ICustomerService
{
    Task<ResponseModel<object>> GetCustomers();

    Task<ResponseModel<object>> GetCustomer(GetCustomerRequest request);

    Task<ResponseModel<object>> RegisterCustomer(CustomerModel request);

    Task<ResponseModel<object>> EditCustomer(CustomerModel request);

    Task<ResponseModel<object>> DeactivateCustomer(string guid);

    Task<ResponseModel<object>> DeleteCustomer(string guid);
}