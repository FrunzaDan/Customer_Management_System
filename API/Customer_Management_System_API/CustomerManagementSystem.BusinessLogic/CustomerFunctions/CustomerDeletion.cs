using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Models;

namespace Customer_Management_System_Library;

public class CustomerDeletion
{
    private readonly IDBUtils _dbUtils;

    public CustomerDeletion(IDBUtils dBUtils)
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