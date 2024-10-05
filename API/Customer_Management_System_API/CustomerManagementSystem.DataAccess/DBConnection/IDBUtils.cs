using CustomerManagementSystem.Domain.Models;

namespace CustomerManagementSystem.DataAccess.DBConnection
{
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
}