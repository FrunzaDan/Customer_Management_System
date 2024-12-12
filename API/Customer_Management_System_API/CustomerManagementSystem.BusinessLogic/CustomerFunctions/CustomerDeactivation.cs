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

    public ResponseModel DeactivateCustomer(string customerGUID)
    {
        var response = new ResponseModel();
        try
        {
            response = _dbUtils.DeactivateCustomer(customerGUID);
        }
        catch (Exception ex)
        {
            response.ResponseCode = 500;
            response.ResponseMessage = ex.ToString();
        }

        return response;
    }
}