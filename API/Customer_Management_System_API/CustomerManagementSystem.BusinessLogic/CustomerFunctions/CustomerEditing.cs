using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Models;

namespace Customer_Management_System_Library;

public class CustomerEditing
{
    private readonly IDBUtils _dbUtils;

    public CustomerEditing(IDBUtils dBUtils)
    {
        _dbUtils = dBUtils;
    }

    public ResponseModel EditCustomerFunction(CustomerModel editCustomerRqst)
    {
        var response = new ResponseModel();
        try
        {
            response = _dbUtils.EditCustomer(editCustomerRqst);
        }
        catch (Exception ex)
        {
            response.ResponseCode = 500;
            response.ResponseMessage = ex.ToString();
        }

        return response;
    }
}