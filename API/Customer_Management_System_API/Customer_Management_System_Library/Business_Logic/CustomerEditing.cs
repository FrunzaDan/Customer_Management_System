using Customer_Management_System_Library.Models;

namespace Customer_Management_System_Library
{
    public class CustomerEditing
    {
        private readonly IDBUtils _dbUtils;

        public CustomerEditing(IDBUtils dBUtils)
        {
            _dbUtils = dBUtils;
        }

        public ResponseModel EditCustomerFunction(CustomerModel editCustomerRqst)
        {
            ResponseModel response = new ResponseModel();
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
}