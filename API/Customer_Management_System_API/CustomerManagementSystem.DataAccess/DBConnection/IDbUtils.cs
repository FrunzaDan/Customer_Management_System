using CustomerManagementSystem.Domain.Models;

namespace CustomerManagementSystem.DataAccess.DBConnection;

public interface IDbUtils
{
    public Task<ResponseModel> RegisterCustomer(CustomerModel customer);

    public Task<CustomerModel> GetCustomer(GetCustomerRequest getCustomerRqst);

    public Task<CustomerListModel> GetCustomers();

    public Task<ResponseModel> EditCustomer(CustomerModel editCustomerRqst);

    public Task<ResponseModel> DeactivateCustomer(string customerGuid);

    public Task<ResponseModel> DeleteCustomer(string customerGuid);

    public Task<MerchantCredentialsCheck> CheckMerchantCredentialsFromDb(MerchantCredentials merchantCredentials);
}