using CustomerManagementSystem.BusinessLogic.AuthFunctions;
using CustomerManagementSystem.BusinessLogic.CustomerFunctions;
using CustomerManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace CustomerManagementSystem.BusinessLogic.Services.Implementation;

public class CustomerService : ICustomerService
{
    private readonly bool _isAuthorized;

    public CustomerService(IHttpContextAccessor httpContextAccessor)
    {
        var jwtValidation = new JwtValidation();
        _isAuthorized = jwtValidation.Authorize(httpContextAccessor.HttpContext, null);
    }

    public async Task<ResponseModel<object>> DeactivateCustomer(string customerGuid)
    {
        if (!_isAuthorized)
            return new ResponseModel<object>
            {
                Status = 403,
                ResponseMessage = "No access rights for this request!"
            };

        var customerDeactivation = new CustomerDeactivation();
        return await customerDeactivation.DeactivateCustomer(customerGuid);
    }

    public async Task<ResponseModel<object>> DeleteCustomer(string customerGuid)
    {
        if (!_isAuthorized)
            return new ResponseModel<object>
            {
                Status = 403,
                ResponseMessage = "No access rights for this request!"
            };

        var customerDeletion = new CustomerDeletion();
        return await customerDeletion.DeleteCustomer(customerGuid);
    }

    public async Task<ResponseModel<object>> EditCustomer(CustomerModel editCustomerRequest)
    {
        if (!_isAuthorized)
            return new ResponseModel<object>
            {
                Status = 403,
                ResponseMessage = "No access rights for this request!"
            };

        var customerEditing = new CustomerEditing();
        return await customerEditing.EditCustomerFunction(editCustomerRequest);
    }

    public async Task<ResponseModel<CustomerModel>> GetCustomer(GetCustomerRequest getCustomerRqst)
    {
        if (!_isAuthorized)
            return new ResponseModel<CustomerModel>
            {
                Status = 403,
                ResponseMessage = "No access rights for this request!"
            };

        var customerGetting = new CustomerGetting();
        return await customerGetting.GetCustomerFunction(getCustomerRqst);
    }

    public async Task<ResponseModel<CustomerListModel>> GetCustomers()
    {
        if (!_isAuthorized)
            return new ResponseModel<CustomerListModel>
            {
                Status = 403,
                ResponseMessage = "No access rights for this request!"
            };

        var customerGetting = new CustomerGetting();
        return await customerGetting.GetCustomersFunction();
    }

    public async Task<ResponseModel<object>> RegisterCustomer(CustomerModel customerRqst)
    {
        if (!_isAuthorized)
            return new ResponseModel<object>
            {
                Status = 403,
                ResponseMessage = "No access rights for this request!"
            };

        var customerRegistration = new CustomerRegistration();
        return await customerRegistration.RegisterCustomerFunction(customerRqst);
    }
}
