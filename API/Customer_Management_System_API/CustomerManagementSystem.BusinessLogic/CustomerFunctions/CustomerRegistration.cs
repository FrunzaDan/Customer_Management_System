using CustomerManagementSystem.BusinessLogic.Validations;
using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Models;

namespace CustomerManagementSystem.BusinessLogic.CustomerFunctions;

public class CustomerRegistration
{
    private readonly IDbUtils _dbUtils;

    public CustomerRegistration(IDbUtils dbUtils)
    {
        _dbUtils = dbUtils;
    }

    public async Task<ResponseModel<object>> RegisterCustomerFunction(CustomerModel customerRqst)
    {
        if (customerRqst.Email is not null && customerRqst.Msisdn is not null)
        {
            if (EmailValidation.ValidateEmail(customerRqst.Email) == false)
                return new ResponseModel<object>(409, "Invalid or empty Email.");

            if (MsisdnValidation.ValidateMsisdn(customerRqst.Msisdn) == false)
                return new ResponseModel<object>(409, "Invalid or empty MSISDN.");
        }
        
        return await _dbUtils.RegisterCustomer(customerRqst);
    }
}