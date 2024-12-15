using System.Data;
using CustomerManagementSystem.BusinessLogic.Configuration;
using CustomerManagementSystem.BusinessLogic.CustomerFunctions;
using CustomerManagementSystem.BusinessLogic.AuthFunctions;
using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace CustomerManagementSystem.BusinessLogic.Services.Implementation;

public class CustomerService : ICustomerService
{
    private readonly IBllConfig _configuration = ServiceLocator.GetServiceFromServiceProvider<IBllConfig>();
    private readonly IDbUtils _dbUtils = ServiceLocator.GetServiceFromServiceProvider<IDbUtils>();
    private readonly bool _isAuthorized;
    
    public CustomerService(IHttpContextAccessor httpContextAccessor)
    {
        JwtValidation jwtValidation = new JwtValidation(_configuration);
        _isAuthorized = jwtValidation.Authorize(httpContextAccessor.HttpContext, null);
    }

    public async Task<ResponseModel> DeactivateCustomer(string customerGuid)
    {
        if (!_isAuthorized)
        {
            return new ResponseModel
            {
                Status = 403,
                ResponseMessage = "No access rights for this request!"
            };
        }

        var customerDeactivation = new CustomerDeactivation(_dbUtils);
        var response = await customerDeactivation.DeactivateCustomer(customerGuid);
        return response;
    }

    public async Task<ResponseModel> DeleteCustomer(string customerGuid)
    {
        if (!_isAuthorized)
        {
            return new ResponseModel
            {
                Status = 403,
                ResponseMessage = "No access rights for this request!"
            };
        }
        
        var customerDeletion = new CustomerDeletion(_dbUtils);
        var response = await customerDeletion.DeleteCustomer(customerGuid);
        return response;
    }

    public async Task<ResponseModel> EditCustomer(CustomerModel editCustomerRequest)
    {
        if (!_isAuthorized)
        {
            return new ResponseModel
            {
                Status = 403,
                ResponseMessage = "No access rights for this request!"
            };
        }
        
        var customerEditing = new CustomerEditing(_dbUtils);
        var response = await customerEditing.EditCustomerFunction(editCustomerRequest);
        return response;
    }

    public async Task<CustomerModel> GetCustomer(GetCustomerRequest getCustomerRqst)
    {
        if (!_isAuthorized)
        {
            return new CustomerModel()
            {
                Status = 403,
                ResponseMessage = "No access rights for this request!"
            };
        }
        
        var customerGetting = new CustomerGetting(_dbUtils);
        var response = await customerGetting.GetCustomerFunction(getCustomerRqst);
        return response;
    }

    public async Task<CustomerListModel> GetCustomers()
    {
        if (!_isAuthorized)
        {
            return new CustomerListModel()
            {
                Status = 403,
                ResponseMessage = "No access rights for this request!"
            };
        }
        var customerGetting = new CustomerGetting(_dbUtils);
        var response = await customerGetting.GetCustomersFunction();
        return response;
    }

    public async Task<ResponseModel> RegisterCustomer(CustomerModel customerRqst)
    {
        if (!_isAuthorized)
        {
            return new ResponseModel()
            {
                Status = 403,
                ResponseMessage = "No access rights for this request!"
            };
        }
        var customerRegistration = new CustomerRegistration(_dbUtils);
        var response = await customerRegistration.RegisterCustomerFunction(customerRqst);
        return response;
    }
}