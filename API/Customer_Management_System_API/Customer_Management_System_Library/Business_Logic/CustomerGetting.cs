using Customer_Management_System_Library.Models;
using Customer_Management_System_Library.Validations;

namespace Customer_Management_System_Library.Functions
{
    public class CustomerGetting
    {
        private readonly IDBUtils _dbUtils; 

        public CustomerGetting(IDBUtils dBUtils)
        {
            _dbUtils = dBUtils;
        }

        public CustomerModel GetCustomerFunction(GetCustomerRequest getCustomerRqst)
        {
            CustomerModel response = new CustomerModel();
            if(getCustomerRqst.searchVariable is null)
            {
                response.ResponseCode = 500;
                return response;
            }
            if (GUIDValidation.ValidateGUID(getCustomerRqst.searchVariable))
            {
                getCustomerRqst.searchOption = 1;
            }
            else if (MSISDNValidation.ValidateMsisdn(getCustomerRqst.searchVariable))
            {
                getCustomerRqst.searchOption = 2;
            }
            else if (EmailValidation.ValidateEmail(getCustomerRqst.searchVariable))
            {
                getCustomerRqst.searchOption = 3;
            }
            else
            {
                response.ResponseMessage = "No valid search variable was provided! Search variables can be GUID, MSISDN or Email!";
                return response;
            }
            try
            {
                response = _dbUtils.GetCustomer(getCustomerRqst);
            }
            catch (Exception ex)
            {
                response.ResponseCode = 500;
                response.ResponseMessage = ex.ToString();
            }
            return response;

        }

        public CustomerListModel GetCustomersFunction()
        {
            CustomerListModel response = new CustomerListModel();

            try
            {
                response = _dbUtils.GetCustomers();
            }
            catch (Exception)
            {
            }
            return response;

        }
    }
}

