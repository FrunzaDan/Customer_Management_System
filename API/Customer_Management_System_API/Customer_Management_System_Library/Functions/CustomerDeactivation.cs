namespace Customer_Management_System_Library;
using Customer_Management_System_Library.Configuration;
using Customer_Management_System_Library.DataAccess;
using Customer_Management_System_Library.Models;
public class CustomerDeactivation
{
    private readonly ICMSConfig _configuration;

    public CustomerDeactivation(ICMSConfig configuration)
    {
        _configuration = configuration;
    }

    public ResponseModel DeactivateCustomer(string customerGUID)
    {
        ResponseModel response = new ResponseModel();
        try
        {
            DBUtils dBUtils = new DBUtils(_configuration);
            response = dBUtils.DeactivateCustomer(customerGUID);
        }
        catch (Exception ex)
        {
            response.ResponseCode = 500;
            response.ResponseMessage = ex.ToString();
        }

        return response;
    }
}
