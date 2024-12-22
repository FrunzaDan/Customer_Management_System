using CustomerManagementSystem.BusinessLogic.Validations;
using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Models;

namespace CustomerManagementSystem.BusinessLogic.CustomerFunctions;

public class CustomerGetting
{
    private readonly IDbUtils _dbUtils = ServiceLocator.GetServiceFromServiceProvider<IDbUtils>();

    public async Task<CustomerModel> GetCustomerFunction(GetCustomerRequest getCustomerRqst)
    {
        if (string.IsNullOrEmpty(getCustomerRqst.SearchVariable)) return new CustomerModel { Status = 404 };

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
            return new CustomerModel
            {
                Status = 400,
                ResponseMessage =
                    "No valid search variable was provided! Search variables can be GUID, MSISDN or Email!"
            };

        try
        {
            return await _dbUtils.GetCustomer(getCustomerRqst);
        }
        catch (Exception ex)
        {
            return new CustomerModel
            {
                Status = 500,
                ResponseMessage = ex.Message
            };
        }
    }

    public async Task<CustomerListModel> GetCustomersFunction()
    {
        return await _dbUtils.GetCustomers();
    }
}