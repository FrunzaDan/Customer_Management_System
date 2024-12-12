using CustomerManagementSystem.BusinessLogic.Validations;
using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Models;

namespace CustomerManagementSystem.BusinessLogic.CustomerFunctions;

public class CustomerRegistration
{
    private readonly IDbUtils _dbUtils;

    public CustomerRegistration(IDbUtils dBUtils)
    {
        _dbUtils = dBUtils;
    }

    public ResponseModel RegisterCustomerFunction(CustomerModel customerRqst)
    {
        var response = new ResponseModel();
        if (customerRqst.Email is not null && customerRqst.Msisdn is not null)
        {
            if (EmailValidation.ValidateEmail(customerRqst.Email) == false)
            {
                response.ResponseCode = 409;
                response.ResponseMessage = "Invalid Email";
                return response;
            }

            if (MSISDNValidation.ValidateMsisdn(customerRqst.Msisdn) == false)
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