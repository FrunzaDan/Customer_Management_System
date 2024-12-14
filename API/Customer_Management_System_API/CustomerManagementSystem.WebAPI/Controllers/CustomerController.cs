using CustomerManagementSystem.BusinessLogic.AuthFunctions;
using CustomerManagementSystem.BusinessLogic.Services;
using CustomerManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CustomerManagementSystem.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerController(IHttpClientFactory httpClientFactory, ICustomerService customerService)
    {
        _customerService = customerService;
    }
    [Route("[action]")]
    [HttpPost]
    public async Task<ResponseModel> RegisterCustomer(CustomerModel customerRqst)
    {
        var response = await _customerService.RegisterCustomer(customerRqst);
        if (response.ResponseCode.HasValue)
        {
            Response.StatusCode = (int)response.ResponseCode;
        }
        return response;
    }

    [Route("[action]")]
    [HttpGet]
    public async Task<CustomerModel> GetCustomer(string searchVariable)
    {
        var getCustomerRqst = new GetCustomerRequest
        {
            SearchVariable = searchVariable
        };
        var response = await _customerService.GetCustomer(getCustomerRqst);
        if (response.ResponseCode.HasValue)
        {
            Response.StatusCode = (int)response.ResponseCode;
        }
        return response;
    }

    [Route("[action]")]
    [HttpGet]
    public async Task<CustomerListModel> GetCustomers()
    {
        var response = await _customerService.GetCustomers();
        if (response.ResponseCode.HasValue)
        {
            Response.StatusCode = (int)response.ResponseCode;
        }
        return response;
    }

    [Route("[action]")]
    [HttpPatch]
    public async Task<ResponseModel> EditCustomer(CustomerModel editCustomerRqst)
    {
        var response = await _customerService.EditCustomer(editCustomerRqst);
        if (response.ResponseCode.HasValue)
        {
            Response.StatusCode = (int)response.ResponseCode;
        }
        return response;
    }

    [Route("[action]")]
    [HttpPatch]
    public async Task<ResponseModel> DeactivateCustomer(string customerGUID)
    {
        var response = await _customerService.DeactivateCustomer(customerGUID);
        if (response.ResponseCode.HasValue)
        {
            Response.StatusCode = (int)response.ResponseCode;
        }
        return response;
    }

    [Route("[action]")]
    [HttpDelete]
    public async Task<ResponseModel> DeleteCustomer(string customerGuid)
    {
        var response = await _customerService.DeleteCustomer(customerGuid);
        
        if (response.ResponseCode.HasValue)
        {
            Response.StatusCode = (int)response.ResponseCode;
        }
        return response;
    }
}