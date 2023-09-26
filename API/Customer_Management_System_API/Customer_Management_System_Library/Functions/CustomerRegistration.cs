using System;
using Azure;
using Customer_Management_System_Library.Configuration;
using Customer_Management_System_Library.DataAccess;
using Customer_Management_System_Library.Models;
using Customer_Management_System_Library.Validations;
using Microsoft.Extensions.Configuration;

namespace Customer_Management_System_Library.Functions
{
    public class CustomerRegistration
    {
        private readonly ICMSConfig _configuration;

        public CustomerRegistration(ICMSConfig configuration)
        {
            _configuration = configuration;
        }

        public ResponseModel RegisterCustomerFunction(CustomerModel customerRqst)
        {
            ResponseModel response = new ResponseModel();
            if (customerRqst.Email is not null && customerRqst.MSISDN is not null)
            {
                if (EmailValidation.ValidateEmail(customerRqst.Email) == false)
                {
                    response.ResponseCode = 409;
                    response.ResponseMessage = "Invalid Email";
                    return response;
                }
                if (MSISDNValidation.ValidateMsisdn(customerRqst.MSISDN) == false)
                {
                    response.ResponseCode = 409;
                    response.ResponseMessage = "Invalid MSISDN";
                    return response;
                }
            }

            try
            {
                DBUtils dBUtils = new DBUtils(_configuration);
                response = dBUtils.RegisterCustomer(customerRqst);
            }
            catch (Exception ex)
            {
                response.ResponseCode = 500;
                response.ResponseMessage = ex.ToString();
            }
            return response;

        }
    }
}

