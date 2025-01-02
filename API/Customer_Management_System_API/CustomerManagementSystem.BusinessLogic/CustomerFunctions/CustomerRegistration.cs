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

    public async Task<ResponseModel<object>> RegisterCustomerFunction(CustomerModel request)
    {
        if (string.IsNullOrEmpty(request.Email) || EmailValidation.ValidateEmail(request.Email) == false)
            return new ResponseModel<object>(404, "Invalid or empty Email.");

        if (string.IsNullOrEmpty(request.Msisdn) || MsisdnValidation.ValidateMsisdn(request.Msisdn) == false)
            return new ResponseModel<object>(404, "Invalid or empty MSISDN.");

        return await _dbUtils.RegisterCustomer(request);
    }
}