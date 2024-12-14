using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Models;

namespace CustomerManagementSystem.BusinessLogic.CustomerFunctions;

public class CustomerDeletion
{
    private readonly IDbUtils _dbUtils;

    public CustomerDeletion(IDbUtils dBUtils)
    {
        _dbUtils = dBUtils;
    }

    public async Task<ResponseModel> DeleteCustomer(string customerGuid)
    {
        var response = new ResponseModel();
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