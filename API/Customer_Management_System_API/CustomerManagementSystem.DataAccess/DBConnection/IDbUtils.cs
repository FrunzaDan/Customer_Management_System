using CustomerManagementSystem.Domain.Models;

namespace CustomerManagementSystem.DataAccess.DBConnection;

public interface IDbUtils
{
    public Task<ResponseModel<object>> RegisterCustomer(CustomerModel customer);
    public Task<ResponseModel<object>> GetCustomer(GetCustomerRequest getCustomerRqst);
    public Task<ResponseModel<object>> GetCustomers();
    public Task<ResponseModel<object>> EditCustomer(CustomerModel editCustomerRqst);
    public Task<ResponseModel<object>> DeactivateCustomer(string customerGuid);
    public Task<ResponseModel<object>> ReactivateCustomer(string customerGuid);
    public Task<ResponseModel<object>> DeleteCustomer(string customerGuid);
    public Task<ResponseModel<object>> CheckMerchantCredentialsFromDb(MerchantCredentials merchantCredentials);
}