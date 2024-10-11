using CustomerManagementSystem.Domain.Models;
using CustomerManagementSystem.DataAccess.DBConnection;

namespace CustomerManagementSystem.BusinessLogic.CustomerFunctions
{
    public class CustomerDeactivation
    {
        private readonly IDBUtils _dbUtils;

        public CustomerDeactivation(IDBUtils dBUtils)
        {
            _dbUtils = dBUtils;
        }

        public ResponseModel DeactivateCustomer(string customerGUID)
        {
            ResponseModel response = new ResponseModel();
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
}