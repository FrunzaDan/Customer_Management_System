using Customer_Management_System_Library.Models;
using Customer_Management_System_Library.Validations;

namespace Customer_Management_System_Library.Functions
{
    public class CustomerRegistration
    {
        private readonly IDBUtils _dbUtils; 

        public CustomerRegistration(IDBUtils dBUtils)
        {
            _dbUtils = dBUtils;
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
                response = _dbUtils.RegisterCustomer(customerRqst);
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

