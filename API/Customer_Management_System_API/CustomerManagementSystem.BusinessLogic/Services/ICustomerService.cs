using CustomerManagementSystem.Domain.Models;

namespace CustomerManagementSystem.BusinessLogic.Services;

public interface ICustomerService
{
    public CustomerListModel GetCustomers();

    public CustomerModel GetCustomer(GetCustomerRequest getCustomerRequest);

    public ResponseModel RegisterCustomerFunction(CustomerModel customerRequest);

    public ResponseModel EditCustomerFunction(CustomerModel editCustomerRequest);

    public ResponseModel DeactivateCustomer(string customerGuid);

    public ResponseModel DeleteCustomer(string customerGuid);
}