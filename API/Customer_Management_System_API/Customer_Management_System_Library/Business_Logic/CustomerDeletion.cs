using Customer_Management_System_Library.Models;
using Customer_Management_System_Library.Configuration;
using Customer_Management_System_Library.DataAccess;

namespace Customer_Management_System_Library
{
    public class CustomerDeletion
    {
        private readonly IDBUtils _dbUtils;

        public CustomerDeletion(IDBUtils dBUtils)
        {
            _dbUtils = dBUtils;
        }

        public ResponseModel DeleteCustomer(string customerGUID)
        {
            ResponseModel response = new ResponseModel();
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
}
