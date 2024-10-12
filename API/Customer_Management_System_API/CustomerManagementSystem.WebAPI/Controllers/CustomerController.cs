using CustomerManagementSystem.BusinessLogic.Services;
using CustomerManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CustomerManagementSystem.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICustomerService _customerService;
        public CustomerController(IHttpClientFactory httpClientFactory, ICustomerService customerService)
        {
            _httpClientFactory = httpClientFactory;
            _customerService = customerService;
        }
        // [Route("[action]")]
        // [HttpPost]
        // public ResponseModel RegisterCustomer(CustomerModel customerRqst)
        // {
        //     ResponseModel response = new ResponseModel();
        //     HttpContext httpContext = HttpContext;
        //     MerchantCredentials clientDetails = new MerchantCredentials();
        //     JWTValidation jwtValidation = new JWTValidation(_configuration);
        //     if (jwtValidation.Authorize(httpContext, null))
        //     {
        //         CustomerRegistration customerRegistration = new CustomerRegistration(_dbUtils);
        //         response = customerRegistration.RegisterCustomerFunction(customerRqst);
        //     }
        //     else
        //     {
        //         response.ResponseCode = 403;
        //         response.ResponseMessage = "No Access Rights!";
        //     }
        //     if (response.ResponseCode is not null)
        //     {
        //         Response.StatusCode = (int)response.ResponseCode;
        //     }
        //     return response;
        // }

        // [Route("[action]")]
        // [HttpGet]
        // public CustomerModel GetCustomer(string searchVariable)
        // {
        //     CustomerModel response = new CustomerModel();

        //     HttpContext httpContext = HttpContext;
        //     MerchantCredentials clientDetails = new MerchantCredentials();
        //     JWTValidation jwtValidation = new JWTValidation(_configuration);
        //     if (jwtValidation.Authorize(httpContext, null))
        //     {
        //         CustomerGetting customerGetting = new CustomerGetting(_dbUtils);
        //         GetCustomerRequest getCustomerRqst = new GetCustomerRequest();
        //         getCustomerRqst.searchVariable = searchVariable;
        //         response = customerGetting.GetCustomerFunction(getCustomerRqst);
        //     }
        //     else
        //     {
        //         response.ResponseCode = 403;
        //         response.ResponseMessage = "No Access Rights!";
        //     }
        //     if (response.ResponseCode is not null)
        //     {
        //         Response.StatusCode = (int)response.ResponseCode;
        //     }
        //     return response;
        // }

        // [Route("[action]")]
        // [HttpGet]
        // public CustomerListModel GetCustomers()
        // {
        //     CustomerListModel response = new CustomerListModel();

        //     HttpContext httpContext = HttpContext;
        //     MerchantCredentials clientDetails = new MerchantCredentials();
        //     JWTValidation jwtValidation = new JWTValidation(_configuration);
        //     if (jwtValidation.Authorize(httpContext, null))
        //     {
        //         CustomerGetting customerGetting = new CustomerGetting(_dbUtils);
        //         GetCustomerRequest getCustomerRqst = new GetCustomerRequest();
        //         response = customerGetting.GetCustomersFunction();
        //     }
        //     else
        //     {
        //         response.ResponseCode = 403;
        //         response.ResponseMessage = "No Access Rights!";
        //     }
        //     if (response.ResponseCode is not null)
        //     {
        //         Response.StatusCode = (int)response.ResponseCode;
        //     }
        //     return response;
        // }

        // [Route("[action]")]
        // [HttpPatch]
        // public ResponseModel EditCustomer(CustomerModel editCustomerRqst)
        // {
        //     ResponseModel response = new ResponseModel();

        //     HttpContext httpContext = HttpContext;
        //     MerchantCredentials clientDetails = new MerchantCredentials();
        //     JWTValidation jwtValidation = new JWTValidation(_configuration);
        //     if (jwtValidation.Authorize(httpContext, null))
        //     {
        //         CustomerEditing customerEditing = new CustomerEditing(_dbUtils);
        //         GetCustomerRequest getCustomerRqst = new GetCustomerRequest();
        //         response = customerEditing.EditCustomerFunction(editCustomerRqst);
        //     }
        //     else
        //     {
        //         response.ResponseCode = 403;
        //         response.ResponseMessage = "No Access Rights!";
        //     }
        //     if (response.ResponseCode is not null)
        //     {
        //         Response.StatusCode = (int)response.ResponseCode;
        //     }
        //     return response;
        // }

        // [Route("[action]")]
        // [HttpPatch]
        // public ResponseModel DeactivateCustomer(string customerGUID)
        // {
        //     ResponseModel response = new ResponseModel();

        //     HttpContext httpContext = HttpContext;
        //     MerchantCredentials clientDetails = new MerchantCredentials();
        //     JWTValidation jwtValidation = new JWTValidation(_configuration);
        //     if (jwtValidation.Authorize(httpContext, null))
        //     {
        //         CustomerDeactivation customerDeactivation = new CustomerDeactivation(_dbUtils);
        //         response = customerDeactivation.DeactivateCustomer(customerGUID);
        //     }
        //     else
        //     {
        //         response.ResponseCode = 403;
        //         response.ResponseMessage = "No Access Rights!";
        //     }
        //     if (response.ResponseCode is not null)
        //     {
        //         Response.StatusCode = (int)response.ResponseCode;
        //     }
        //     return response;
        // }

        // [Route("[action]")]
        // [HttpDelete]
        // public CustomerListModel DeleteCustomer(string customerGUID)
        // {
        //     CustomerListModel response = new CustomerListModel();

        //     HttpContext httpContext = HttpContext;
        //     MerchantCredentials clientDetails = new MerchantCredentials();
        //     JWTValidation jwtValidation = new JWTValidation(_configuration);
        //     if (jwtValidation.Authorize(httpContext, null))
        //     {
        //         CustomerDeletion customerDeletion = new CustomerDeletion(_dbUtils);
        //         GetCustomerRequest getCustomerRqst = new GetCustomerRequest();
        //         customerDeletion.DeleteCustomer(customerGUID);
        //     }
        //     else
        //     {
        //         response.ResponseCode = 403;
        //         response.ResponseMessage = "No Access Rights!";
        //     }
        //     if (response.ResponseCode is not null)
        //     {
        //         Response.StatusCode = (int)response.ResponseCode;
        //     }
        //     return response;
        // }
    }
}