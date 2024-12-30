using CustomerManagementSystem.BusinessLogic.AuthFunctions;
using CustomerManagementSystem.BusinessLogic.CustomerFunctions;
using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Configuration;
using CustomerManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace CustomerManagementSystem.BusinessLogic.Services.Implementation;

public class CustomerService : ICustomerService
{
    private readonly IDbUtils _dbUtils;
    private readonly bool _isAuthorized;

    public CustomerService(IHttpContextAccessor httpContextAccessor, IDbUtils dbUtils, IAppSettingsConfig appSettingsConfig)
    {
        _dbUtils = dbUtils;

        var jwtValidation = new JwtValidation(appSettingsConfig);
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

        var customerDeactivation = new CustomerDeactivation(_dbUtils);
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

        var customerDeletion = new CustomerDeletion(_dbUtils);
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

        var customerEditing = new CustomerEditing(_dbUtils);
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

        var customerGetting = new CustomerGetting(_dbUtils);
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

        var customerGetting = new CustomerGetting(_dbUtils);
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

        var customerRegistration = new CustomerRegistration(_dbUtils);
        return await customerRegistration.RegisterCustomerFunction(customerRqst);
    }
}