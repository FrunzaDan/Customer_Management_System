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
    public async Task<ResponseModel<object>> DeactivateCustomer(string customerGuid, string merchantId) =>
        await customerActivation.DeactivateCustomer(customerGuid, merchantId);

    public async Task<ResponseModel<object>> ReactivateCustomer(string customerGuid, string merchantId) =>
        await customerActivation.ReactivateCustomer(customerGuid, merchantId);

    public async Task<ResponseModel<object>> DeleteCustomer(string customerGuid, string merchantId) =>
        await customerDeletion.DeleteCustomer(customerGuid, merchantId);

    public async Task<ResponseModel<object>> EditCustomer(CustomerModel editCustomerRequest, string merchantId) =>
        await customerEditing.EditCustomerFunction(editCustomerRequest, merchantId);

    public async Task<ResponseModel<object>> GetCustomer(GetCustomerRequest getCustomerRqst) =>
        await customerGetting.GetCustomerFunction(getCustomerRqst);

    public async Task<ResponseModel<object>> GetCustomerAuditLog(string customerGuid) =>
        await customerGetting.GetCustomerAuditLogFunction(customerGuid);

    public async Task<ResponseModel<object>> GetCustomers(GetCustomersRequest request) =>
        await customerGetting.GetCustomersFunction(request);

    public async Task<ResponseModel<object>> GetCustomersForExport(ExportCustomersRequest request) =>
        await customerGetting.GetCustomersForExportFunction(request);

    public async Task<ResponseModel<object>> RegisterCustomer(CustomerModel customerRqst, string merchantId) =>
        await customerRegistration.RegisterCustomerFunction(customerRqst, merchantId);
}
