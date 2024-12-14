using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Models;

namespace CustomerManagementSystem.BusinessLogic.CustomerFunctions;

public class CustomerDeactivation
{
    private readonly IDbUtils _dbUtils;

    public CustomerDeactivation(IDbUtils dBUtils)
    {
        _dbUtils = dBUtils;
    }

    public async Task<ResponseModel> DeactivateCustomer(string customerGuid)
    {
        var response = new ResponseModel();
        try
        {
            response = await _dbUtils.DeactivateCustomer(customerGuid);
        }
        catch (Exception ex)
        {
            response.ResponseCode = 500;
            response.ResponseMessage = ex.ToString();
        }

        return response;
    }
}