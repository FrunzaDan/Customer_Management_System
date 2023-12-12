namespace Customer_Management_System_Library;

using Customer_Management_System_Library.Models;

public interface IDBUtils
{
    public ResponseModel RegisterCustomer(CustomerModel customer);

    public CustomerModel GetCustomer(GetCustomerRequest getCustomerRqst);

    public CustomerListModel GetCustomers();

    public ResponseModel EditCustomer(CustomerModel editCustomerRqst);

    public ResponseModel DeactivateCustomer(string customerGUID);

    public ResponseModel DeleteCustomer(string customerGUID);

    public bool CheckMerchantCredentialsFromDB(MerchantCredentials merchantCredentials);
}