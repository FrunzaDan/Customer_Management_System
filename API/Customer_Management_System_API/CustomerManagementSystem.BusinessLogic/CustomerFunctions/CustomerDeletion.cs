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

    public ResponseModel DeleteCustomer(string customerGUID)
    {
        var response = new ResponseModel();
        try
        {
            response = _dbUtils.DeleteCustomer(customerGUID);
        }
        catch (Exception ex)
        {
            response.ResponseCode = 500;
            response.ResponseMessage = ex.ToString();
        }

        return response;
    }
}