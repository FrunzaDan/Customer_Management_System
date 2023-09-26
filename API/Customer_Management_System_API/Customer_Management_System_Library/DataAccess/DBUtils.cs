using System.Data;
using Customer_Management_System_Library.Configuration;
using Customer_Management_System_Library.Helpers;
using Customer_Management_System_Library.Models;
using Microsoft.Data.SqlClient;

namespace Customer_Management_System_Library.DataAccess
{
    public class DBUtils
    {
        public readonly ICMSConfig _configuration;

        public DBUtils(ICMSConfig configuration)
        {
            _configuration = configuration;
        }

        public ResponseModel RegisterCustomer(CustomerModel customer)
        {
            ResponseModel response = new ResponseModel();
            int returnValue = 0;

            try
            {
                string newGuid = Guid.NewGuid().ToString();
                SqlConnection sqlConnection = new SqlConnection();
                sqlConnection.ConnectionString = _configuration.CustomerManagementSystemDB;
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandText = "dbo.usp_createCustomer";
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    sqlCommand.Parameters.Add(new SqlParameter("@var_Guid", newGuid));
                    sqlCommand.Parameters.Add(new SqlParameter("@var_FirstName", customer.FirstName));
                    sqlCommand.Parameters.Add(new SqlParameter("@var_LastName", customer.LastName));
                    sqlCommand.Parameters.Add(new SqlParameter("@var_Email", customer.Email));
                    sqlCommand.Parameters.Add(new SqlParameter("@var_MSISDN", customer.MSISDN));
                    sqlCommand.Parameters.Add(new SqlParameter("@var_Gender", customer.Gender));
                    sqlCommand.Parameters.Add(new SqlParameter("@var_Birthdate", customer.Birthdate));

                    AddressModel address = customer.Address;
                    if (address is not null)
                    {
                        sqlCommand.Parameters.Add(new SqlParameter("@var_Country", address.Country));
                        sqlCommand.Parameters.Add(new SqlParameter("@var_County", address.County));
                        sqlCommand.Parameters.Add(new SqlParameter("@var_Town", address.Town));
                        sqlCommand.Parameters.Add(new SqlParameter("@var_ZIP", address.ZIP));
                        sqlCommand.Parameters.Add(new SqlParameter("@var_Street", address.Street));
                        sqlCommand.Parameters.Add(new SqlParameter("@var_Number", address.Number));
                    }

                    var sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        try
                        {
                            returnValue = Int32.Parse(sqlDataReader["ReturnValue"].ToString());

                        }
                        catch
                        {
                        }
                        if (returnValue != 0 && returnValue != 200)
                        {

                            response.ResponseCode = 409;
                            response.ResponseMessage = Mappings.SQLResponseDictionary[returnValue];

                        }
                        else if (returnValue == 200)
                        {
                            response.ResponseCode = returnValue;
                            response.ResponseMessage = Mappings.SQLResponseDictionary[returnValue];
                        }
                        else
                        {
                            response.ResponseCode = 500;
                            response.ResponseMessage = "Failed to connect to DB!";
                        }

                    }

                    sqlDataReader.Close();
                    sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                response.ResponseCode = 500;
                response.ResponseMessage = ex.ToString();
            }

            return response;
        }

        public CustomerModel GetCustomer(GetCustomerRequest getCustomerRqst)
        {
            CustomerModel customerModel = new CustomerModel();
            customerModel.Address = new AddressModel();

            try
            {
                SqlConnection sqlConnection = new SqlConnection();
                sqlConnection.ConnectionString = _configuration.CustomerManagementSystemDB;
                sqlConnection.Open();

                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandText = "dbo.usp_getCustomer";
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@var_SearchOption", getCustomerRqst.searchOption));
                    sqlCommand.Parameters.Add(new SqlParameter("@var_SearchVariable", getCustomerRqst.searchVariable));
                    var sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        try
                        {
                            customerModel.Guid = sqlDataReader["PK_customer_guid"].ToString();
                        }
                        catch { }
                        try
                        {
                            customerModel.FirstName = sqlDataReader["first_name"].ToString();
                        }
                        catch { }
                        try
                        {
                            customerModel.LastName = sqlDataReader["last_name"].ToString();
                        }
                        catch { }
                        try
                        {
                            customerModel.Email = sqlDataReader["email"].ToString();
                        }
                        catch { }
                        try
                        {
                            customerModel.MSISDN = sqlDataReader["msisdn"].ToString();
                        }
                        catch { }
                        try
                        {
                            customerModel.Gender = Int32.Parse(sqlDataReader["gender"].ToString());
                        }
                        catch { }
                        try
                        {
                            customerModel.Birthdate = sqlDataReader["birthDate"].ToString();
                        }
                        catch { }
                        try
                        {
                            customerModel.CustomerStatus = Int32.Parse(sqlDataReader["customer_Status"].ToString());
                        }
                        catch { }
                        try
                        {
                            customerModel.Address.Country = sqlDataReader["country"].ToString();
                        }
                        catch { }
                        try
                        {
                            customerModel.Address.County = sqlDataReader["county"].ToString();
                        }
                        catch { }
                        try
                        {
                            customerModel.Address.ZIP = sqlDataReader["zip_code"].ToString();
                        }
                        catch { }
                        try
                        {
                            customerModel.Address.Town = sqlDataReader["town"].ToString();
                        }
                        catch { }
                        try
                        {
                            customerModel.Address.Street = sqlDataReader["street"].ToString();
                        }
                        catch { }
                        try
                        {
                            customerModel.Address.Number = sqlDataReader["number"].ToString();
                        }
                        catch { }
                    }
                    sqlDataReader.Close();
                    sqlConnection.Close();
                    if (!String.IsNullOrEmpty(customerModel.Guid))
                    {
                        customerModel.ResponseCode = 200;
                        customerModel.ResponseMessage = "Customer found in the DB!";
                    }
                    else
                    {
                        customerModel.ResponseCode = 404;
                        customerModel.ResponseMessage = "Customer was not found in the DB!";
                    }

                }

            }
            catch (Exception ex)
            {
                customerModel.ResponseCode = 500;
                customerModel.ResponseMessage = ex.ToString();
            }

            return customerModel;
        }

        public ResponseModel DeactivateCustomer(string customerGUID)
        {
            ResponseModel responseModel = new ResponseModel();

            try
            {
                SqlConnection sqlConnection = new SqlConnection();
                sqlConnection.ConnectionString = _configuration.CustomerManagementSystemDB;
                sqlConnection.Open();

                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandText = "dbo.usp_deactivateCustomer";
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@var_Guid", customerGUID));
                    var sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        try
                        {
                            if (Int32.Parse(sqlDataReader["customer_Status"].ToString()) == 1903)
                            {
                                responseModel.ResponseCode = 200;
                                responseModel.ResponseMessage = "Customer deactivated successfully!";
                            }
                            else
                            {
                                responseModel.ResponseCode = 500;
                                responseModel.ResponseMessage = "Could not read customer status code!";
                            }
                        }
                        catch (Exception ex)
                        {
                            responseModel.ResponseCode = 500;
                            responseModel.ResponseMessage = ex.ToString();
                        }

                    }
                    sqlDataReader.Close();
                    sqlConnection.Close();
                }

            }
            catch (Exception ex)
            {
                responseModel.ResponseCode = 500;
                responseModel.ResponseMessage = ex.ToString();
            }

            return responseModel;
        }

        public ResponseModel DeleteCustomer(string customerGUID)
        {
            ResponseModel responseModel = new ResponseModel();

            try
            {
                SqlConnection sqlConnection = new SqlConnection();
                sqlConnection.ConnectionString = _configuration.CustomerManagementSystemDB;
                sqlConnection.Open();

                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandText = "dbo.usp_deleteCustomer";
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@var_Guid", customerGUID));
                    var sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        try
                        {
                            if (Int32.Parse(sqlDataReader["customer_Status"].ToString()) == 1903)
                            {
                                responseModel.ResponseCode = 200;
                                responseModel.ResponseMessage = "Customer deactivated successfully!";
                            }
                            else
                            {
                                responseModel.ResponseCode = 500;
                                responseModel.ResponseMessage = "Could not read customer status code!";
                            }
                        }
                        catch (Exception ex)
                        {
                            responseModel.ResponseCode = 500;
                            responseModel.ResponseMessage = ex.ToString();
                        }

                    }
                    sqlDataReader.Close();
                    sqlConnection.Close();
                }

            }
            catch (Exception ex)
            {
                responseModel.ResponseCode = 500;
                responseModel.ResponseMessage = ex.ToString();
            }

            return responseModel;
        }

        public CustomerListModel GetCustomers()
        {

            CustomerListModel customerListResponse = new CustomerListModel();
            List<CustomerModel> customerList = new List<CustomerModel>();

            try
            {
                SqlConnection sqlConnection = new SqlConnection();
                sqlConnection.ConnectionString = _configuration.CustomerManagementSystemDB;
                sqlConnection.Open();

                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandText = "dbo.usp_getCustomers";
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    var sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        CustomerModel customerModel = new CustomerModel();
                        customerModel.Address = new AddressModel();

                        try
                        {
                            customerModel.Guid = sqlDataReader["PK_customer_guid"].ToString();
                        }
                        catch { }
                        try
                        {
                            customerModel.FirstName = sqlDataReader["first_name"].ToString();
                        }
                        catch { }
                        try
                        {
                            customerModel.LastName = sqlDataReader["last_name"].ToString();
                        }
                        catch { }
                        try
                        {
                            customerModel.Email = sqlDataReader["email"].ToString();
                        }
                        catch { }
                        try
                        {
                            customerModel.MSISDN = sqlDataReader["msisdn"].ToString();
                        }
                        catch { }
                        try
                        {
                            customerModel.Gender = Int32.Parse(sqlDataReader["gender"].ToString());
                        }
                        catch { }
                        try
                        {
                            customerModel.Birthdate = sqlDataReader["birthDate"].ToString();
                        }
                        catch { }
                        try
                        {
                            customerModel.CustomerStatus = Int32.Parse(sqlDataReader["customer_Status"].ToString());
                        }
                        catch { }
                        try
                        {
                            customerModel.Address.Country = sqlDataReader["country"].ToString();
                        }
                        catch { }
                        try
                        {
                            customerModel.Address.County = sqlDataReader["county"].ToString();
                        }
                        catch { }
                        try
                        {
                            customerModel.Address.ZIP = sqlDataReader["zip_code"].ToString();
                        }
                        catch { }
                        try
                        {
                            customerModel.Address.Town = sqlDataReader["town"].ToString();
                        }
                        catch { }
                        try
                        {
                            customerModel.Address.Street = sqlDataReader["street"].ToString();
                        }
                        catch { }
                        try
                        {
                            customerModel.Address.Number = sqlDataReader["number"].ToString();
                        }
                        catch { }

                        customerList.Add(customerModel);
                    }
                    sqlDataReader.Close();
                    sqlConnection.Close();
                    customerListResponse.ResponseCode = 200;
                    customerListResponse.ResponseMessage = "Customers found in DB!";
                    customerListResponse.customerList = customerList;
                }

            }
            catch (Exception ex)
            {
                customerListResponse.ResponseCode = 500;
                customerListResponse.ResponseMessage = ex.ToString();
            }

            return customerListResponse;
        }

        public bool CheckMerchantCredentialsFromDB(MerchantCredentials merchantCredentials)
        {
            bool isValid = false;

            try
            {
                SqlConnection sqlConnection = new SqlConnection();
                sqlConnection.ConnectionString = _configuration.CustomerManagementSystemDB;
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandText = "dbo.usp_checkMerchantCredentials";
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@var_MerchantID", merchantCredentials.MerchantID));
                    sqlCommand.Parameters.Add(new SqlParameter("@var_MerchantPassword", merchantCredentials.MerchantPassword));
                    var sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        try
                        {
                            if (Int32.Parse(sqlDataReader["merchant_role"].ToString()) == 1801)
                            {
                                isValid = true;
                            }
                        }
                        catch
                        {
                            isValid = false;
                        }
                    }
                    sqlDataReader.Close();
                    sqlConnection.Close();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                isValid = false;
            }

            return isValid;
        }
    }
}

