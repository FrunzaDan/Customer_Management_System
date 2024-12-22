using CustomerManagementSystem.BusinessLogic.Validations;
using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Models;

namespace CustomerManagementSystem.BusinessLogic.CustomerFunctions;

public class CustomerRegistration
{
    private readonly IDbUtils _dbUtils;

    public CustomerRegistration()
    {
        _dbUtils = ServiceLocator.GetServiceFromServiceProvider<IDbUtils>();
    }

    public async Task<ResponseModel> RegisterCustomerFunction(CustomerModel customerRqst)
    {
        var response = new ResponseModel();
        if (customerRqst.Email is not null && customerRqst.Msisdn is not null)
        {
            if (EmailValidation.ValidateEmail(customerRqst.Email) == false)
            {
                response.Status = 409;
                response.ResponseMessage = "Invalid or empty Email";
                return response;
            }

            if (MsisdnValidation.ValidateMsisdn(customerRqst.Msisdn) == false)
            {
                response.Status = 409;
                response.ResponseMessage = "Invalid or empty MSISDN";
                return response;
            }
        }

        try
        {
            response = await _dbUtils.RegisterCustomer(customerRqst);
        }
        catch (Exception ex)
        {
            response.Status = 500;
            response.ResponseMessage = ex.ToString();
        }

        return response;
    }
}