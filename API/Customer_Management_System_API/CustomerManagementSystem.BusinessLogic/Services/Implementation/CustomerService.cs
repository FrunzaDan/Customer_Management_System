using CustomerManagementSystem.BusinessLogic.CustomerFunctions;
using CustomerManagementSystem.Domain.Models;

namespace CustomerManagementSystem.BusinessLogic.Services.Implementation;

public class CustomerService(
    CustomerRegistration customerRegistration,
    CustomerGetting customerGetting,
    CustomerEditing customerEditing,
    CustomerActivation customerActivation,
    CustomerDeletion customerDeletion)
    : ICustomerService
{
    public async Task<ResponseModel<object>> DeactivateCustomer(string customerGuid) =>
        await customerActivation.DeactivateCustomer(customerGuid);

    public async Task<ResponseModel<object>> ReactivateCustomer(string customerGuid) =>
        await customerActivation.ReactivateCustomer(customerGuid);

    public async Task<ResponseModel<object>> DeleteCustomer(string customerGuid) =>
        await customerDeletion.DeleteCustomer(customerGuid);

    public async Task<ResponseModel<object>> EditCustomer(CustomerModel editCustomerRequest) =>
        await customerEditing.EditCustomerFunction(editCustomerRequest);

    public async Task<ResponseModel<object>> GetCustomer(GetCustomerRequest getCustomerRqst) =>
        await customerGetting.GetCustomerFunction(getCustomerRqst);

    public async Task<ResponseModel<object>> GetCustomers(GetCustomersRequest request) =>
        await customerGetting.GetCustomersFunction(request);

    public async Task<ResponseModel<object>> RegisterCustomer(CustomerModel customerRqst) =>
        await customerRegistration.RegisterCustomerFunction(customerRqst);
}
