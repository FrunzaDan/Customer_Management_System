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

    public async Task<ResponseModel<object>> GetCustomerFunction(GetCustomerRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.SearchVariable))
            return new ResponseModel<object>(404, "Search variable is required.");

        request.SearchOption = DetermineSearchOption(request.SearchVariable);

        if (request.SearchOption == 0)
            return new ResponseModel<object>(404,
                "No valid search variable was provided! It must be a GUID, MSISDN, or Email.");

        return await _dbUtils.GetCustomer(request);
    }

    public async Task<ResponseModel<object>> GetCustomersFunction()
    {
        return await _dbUtils.GetCustomers();
    }

    private static int DetermineSearchOption(string searchVariable)
    {
        return GuidValidation.ValidateGuid(searchVariable) ? 1 :
            MsisdnValidation.ValidateMsisdn(searchVariable) ? 2 :
            EmailValidation.ValidateEmail(searchVariable) ? 3 :
            0;
    }
}