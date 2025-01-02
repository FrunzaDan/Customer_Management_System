using CustomerManagementSystem.BusinessLogic.Validations;
using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Models;

namespace CustomerManagementSystem.BusinessLogic.CustomerFunctions;

public class CustomerGetting
{
    private readonly IDbUtils _dbUtils;

    public CustomerGetting(IDbUtils dbUtils)
    {
        _dbUtils = dbUtils;
    }

    public async Task<ResponseModel<object>> GetCustomerFunction(GetCustomerRequest getCustomerRqst)
    {
        if (string.IsNullOrEmpty(getCustomerRqst.SearchVariable))
            return new ResponseModel<object>(404, "Search variable is required.");

        var searchOptions = new (Func<string, bool> validation, int searchOption)[]
        {
            (GuidValidation.ValidateGuid, 1),
            (MsisdnValidation.ValidateMsisdn, 2),
            (EmailValidation.ValidateEmail, 3)
        };

        foreach (var (validation, searchOption) in searchOptions)
            if (validation(getCustomerRqst.SearchVariable))
            {
                getCustomerRqst.SearchOption = searchOption;
                break;
            }

        if (getCustomerRqst.SearchOption == 0)
            return new ResponseModel<object>
            {
                Status = 400,
                ResponseMessage = "No valid search variable was provided! It must be GUID, MSISDN, or Email."
            };

        return await _dbUtils.GetCustomer(getCustomerRqst);
    }

    public async Task<ResponseModel<object>> GetCustomersFunction()
    {
        return await _dbUtils.GetCustomers();
    }
}