using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Models;

namespace CustomerManagementSystem.BusinessLogic.CustomerFunctions;

public class CustomerDeletion
{
    private readonly IDbUtils _dbUtils;

    public CustomerDeletion()
    {
        _dbUtils = ServiceLocator.GetServiceFromServiceProvider<IDbUtils>();
    }

    public async Task<ResponseModel<object>> DeleteCustomer(string customerGuid)
    {
        var response = new ResponseModel<object>();
        try
        {
            response = await _dbUtils.DeleteCustomer(customerGuid);
        }
        catch (Exception ex)
        {
            response.Status = 500;
            response.ResponseMessage = ex.ToString();
        }

        return response;
    }
}