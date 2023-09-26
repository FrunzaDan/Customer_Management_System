using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Customer_Management_System_Library.DataAccess;
using Customer_Management_System_Library.Models;
using Customer_Management_System_Library.Configuration;
using Customer_Management_System_Library.Auth;
using Customer_Management_System_Library.Functions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Customer_Management_System_Library;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Customer_Management_System_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICMSConfig _configuration;

        public CustomerController(ICMSConfig configuration)
        {
            _configuration = configuration;
        }

        [Route("[action]")]
        [HttpPost]
        public ResponseModel RegisterCustomer(CustomerModel customerRqst)
        {
            ResponseModel response = new ResponseModel();
            HttpContext httpContext = HttpContext;
            MerchantCredentials clientDetails = new MerchantCredentials();
            JWTValidation jwtValidation = new JWTValidation(_configuration);
            if (jwtValidation.Authorization(httpContext, null))
            {
                CustomerRegistration customerRegistration = new CustomerRegistration(_configuration);
                response = customerRegistration.RegisterCustomerFunction(customerRqst);
            }
            else
            {
                response.ResponseCode = 403;
                response.ResponseMessage = "No Access Rights!";
            }
            if (response.ResponseCode is not null)
            {
                Response.StatusCode = (int)response.ResponseCode;
            }
            return response;
        }

        [Route("[action]")]
        [HttpGet]
        public CustomerModel GetCustomer(string searchVariable)
        {
            CustomerModel response = new CustomerModel();

            HttpContext httpContext = HttpContext;
            MerchantCredentials clientDetails = new MerchantCredentials();
            JWTValidation jwtValidation = new JWTValidation(_configuration);
            if (jwtValidation.Authorization(httpContext, null))
            {
                CustomerGetting customerGetting = new CustomerGetting(_configuration);
                GetCustomerRequest getCustomerRqst = new GetCustomerRequest();
                getCustomerRqst.searchVariable = searchVariable;
                response = customerGetting.GetCustomerFunction(getCustomerRqst);
            }
            else
            {
                response.ResponseCode = 403;
                response.ResponseMessage = "No Access Rights!";
            }
            if (response.ResponseCode is not null)
            {
                Response.StatusCode = (int)response.ResponseCode;
            }
            return response;
        }

        [Route("[action]")]
        [HttpGet]
        public CustomerListModel GetCustomers()
        {
            CustomerListModel response = new CustomerListModel();

            HttpContext httpContext = HttpContext;
            MerchantCredentials clientDetails = new MerchantCredentials();
            JWTValidation jwtValidation = new JWTValidation(_configuration);
            if (jwtValidation.Authorization(httpContext, null))
            {
                CustomerGetting customerGetting = new CustomerGetting(_configuration);
                GetCustomerRequest getCustomerRqst = new GetCustomerRequest();
                response = customerGetting.GetCustomersFunction();
            }
            else
            {
                response.ResponseCode = 403;
                response.ResponseMessage = "No Access Rights!";
            }
            if (response.ResponseCode is not null)
            {
                Response.StatusCode = (int)response.ResponseCode;
            }
            return response;
        }

        [Route("[action]")]
        [HttpPatch]
        public CustomerListModel EditCustomer()
        {
            CustomerListModel response = new CustomerListModel();

            HttpContext httpContext = HttpContext;
            MerchantCredentials clientDetails = new MerchantCredentials();
            JWTValidation jwtValidation = new JWTValidation(_configuration);
            if (jwtValidation.Authorization(httpContext, null))
            {
                CustomerEditing customerEditing = new CustomerEditing(_configuration);
                GetCustomerRequest getCustomerRqst = new GetCustomerRequest();
                customerEditing.EditCustomerFunction();
            }
            else
            {
                response.ResponseCode = 403;
                response.ResponseMessage = "No Access Rights!";
            }
            if (response.ResponseCode is not null)
            {
                Response.StatusCode = (int)response.ResponseCode;
            }
            return response;
        }

        [Route("[action]")]
        [HttpPatch]
        public ResponseModel DeactivateCustomer(string customerGUID)
        {
            ResponseModel response = new ResponseModel();

            HttpContext httpContext = HttpContext;
            MerchantCredentials clientDetails = new MerchantCredentials();
            JWTValidation jwtValidation = new JWTValidation(_configuration);
            if (jwtValidation.Authorization(httpContext, null))
            {
                CustomerDeactivation customerDeactivation = new CustomerDeactivation(_configuration);
                response = customerDeactivation.DeactivateCustomer(customerGUID);
            }
            else
            {
                response.ResponseCode = 403;
                response.ResponseMessage = "No Access Rights!";
            }
            if (response.ResponseCode is not null)
            {
                Response.StatusCode = (int)response.ResponseCode;
            }
            return response;
        }

        [Route("[action]")]
        [HttpDelete]
        public CustomerListModel DeleteCustomer()
        {
            CustomerListModel response = new CustomerListModel();

            HttpContext httpContext = HttpContext;
            MerchantCredentials clientDetails = new MerchantCredentials();
            JWTValidation jwtValidation = new JWTValidation(_configuration);
            if (jwtValidation.Authorization(httpContext, null))
            {
                CustomerEditing customerEditing = new CustomerEditing(_configuration);
                GetCustomerRequest getCustomerRqst = new GetCustomerRequest();
                customerEditing.EditCustomerFunction();
            }
            else
            {
                response.ResponseCode = 403;
                response.ResponseMessage = "No Access Rights!";
            }
            if (response.ResponseCode is not null)
            {
                Response.StatusCode = (int)response.ResponseCode;
            }
            return response;
        }
    }
}