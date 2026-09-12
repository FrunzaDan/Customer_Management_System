using CustomerManagementSystem.Domain.Models;

namespace CustomerManagementSystem.BusinessLogic.Services;

public interface ICustomerService
{
    Task<ResponseModel<object>> GetCustomers(GetCustomersRequest request);

    Task<ResponseModel<object>> GetCustomer(GetCustomerRequest request);

    Task<ResponseModel<object>> GetCustomerAuditLog(string customerGuid);

    Task<ResponseModel<object>> RegisterCustomer(CustomerModel request, string merchantId);

    Task<ResponseModel<object>> EditCustomer(CustomerModel request, string merchantId);

    Task<ResponseModel<object>> DeactivateCustomer(string guid, string merchantId);

    Task<ResponseModel<object>> ReactivateCustomer(string guid, string merchantId);

    Task<ResponseModel<object>> DeleteCustomer(string guid, string merchantId);
}