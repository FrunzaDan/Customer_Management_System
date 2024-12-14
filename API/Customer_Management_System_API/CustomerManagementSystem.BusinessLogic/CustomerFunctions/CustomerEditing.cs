using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Models;

namespace CustomerManagementSystem.BusinessLogic.CustomerFunctions;

public class CustomerEditing
{
    private readonly IDbUtils _dbUtils;

    public CustomerEditing(IDbUtils dBUtils)
    {
        _dbUtils = dBUtils;
    }

    public async Task<ResponseModel> EditCustomerFunction(CustomerModel editCustomerRqst)
    {
        var response = new ResponseModel();
        try
        {
            response = await _dbUtils.EditCustomer(editCustomerRqst);
        }
        catch (Exception ex)
        {
            response.ResponseCode = 500;
            response.ResponseMessage = ex.ToString();
        }

        return response;
    }
}