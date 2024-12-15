using CustomerManagementSystem.BusinessLogic.Validations;
using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Models;

namespace CustomerManagementSystem.BusinessLogic.CustomerFunctions;

public class CustomerGetting
{
    private readonly IDbUtils _dbUtils;

    public CustomerGetting()
    {
        _dbUtils = ServiceLocator.GetServiceFromServiceProvider<IDbUtils>();
    }

    public async Task<CustomerModel> GetCustomerFunction(GetCustomerRequest getCustomerRqst)
    {
        var response = new CustomerModel();
        if (getCustomerRqst.SearchVariable is null)
        {
            response.Status = 500;
            return response;
        }

        if (GUIDValidation.ValidateGUID(getCustomerRqst.SearchVariable))
        {
            getCustomerRqst.SearchOption = 1;
        }
        else if (MSISDNValidation.ValidateMsisdn(getCustomerRqst.SearchVariable))
        {
            getCustomerRqst.SearchOption = 2;
        }
        else if (EmailValidation.ValidateEmail(getCustomerRqst.SearchVariable))
        {
            getCustomerRqst.SearchOption = 3;
        }
        else
        {
            response.ResponseMessage =
                "No valid search variable was provided! Search variables can be GUID, MSISDN or Email!";
            return response;
        }

        try
        {
            response = await _dbUtils.GetCustomer(getCustomerRqst);
        }
        catch (Exception ex)
        {
            response.Status = 500;
            response.ResponseMessage = ex.ToString();
        }

        return response;
    }

    public async Task<CustomerListModel> GetCustomersFunction()
    {
        var response = new CustomerListModel();

        try
        {
            response = await _dbUtils.GetCustomers();
        }
        catch (Exception)
        {
        }

        return response;
    }
}