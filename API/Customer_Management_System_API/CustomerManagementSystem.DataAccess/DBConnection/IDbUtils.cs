using CustomerManagementSystem.Domain.Models;

namespace CustomerManagementSystem.DataAccess.DBConnection;

public interface IDbUtils
{
    public ResponseModel RegisterCustomer(CustomerModel customer);

    public CustomerModel GetCustomer(GetCustomerRequest getCustomerRqst);

    public CustomerListModel GetCustomers();

    public ResponseModel EditCustomer(CustomerModel editCustomerRqst);

    public ResponseModel DeactivateCustomer(string customerGuid);

    public ResponseModel DeleteCustomer(string customerGuid);

    public Task<MerchantCredentialsCheck> CheckMerchantCredentialsFromDb(MerchantCredentials merchantCredentials);
}