using Customer_Management_System_Library.Configuration;
using Customer_Management_System_Library.Helpers;
using Customer_Management_System_Library.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Customer_Management_System_Library.DataAccess
{
    public class DBUtils : IDBUtils
    {
        public readonly ICMSConfig _configuration;

        public SqlConnection _sqlConnection;

        public DBUtils(ICMSConfig configuration)
        {
            _configuration = configuration;

            CurrentSQLConnection currentSQLConnection = new CurrentSQLConnection(_configuration);
            _sqlConnection = currentSQLConnection.CreateCurrentSqlConnection();
        }

        public ResponseModel RegisterCustomer(CustomerModel customer)
        {
            ResponseModel response = new ResponseModel();
            int returnValue = 0;

            try
            {
                string newGuid = Guid.NewGuid().ToString();
                _sqlConnection.Open();

                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = _sqlConnection;
                    sqlCommand.CommandText = "dbo.usp_createCustomer";
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    sqlCommand.Parameters.Add(new SqlParameter("@var_Guid", newGuid));
                    sqlCommand.Parameters.Add(new SqlParameter("@var_FirstName", customer.FirstName));
                    sqlCommand.Parameters.Add(new SqlParameter("@var_LastName", customer.LastName));
                    sqlCommand.Parameters.Add(new SqlParameter("@var_Email", customer.Email));
                    sqlCommand.Parameters.Add(new SqlParameter("@var_MSISDN", customer.MSISDN));
                    sqlCommand.Parameters.Add(new SqlParameter("@var_Gender", customer.Gender));
                    sqlCommand.Parameters.Add(new SqlParameter("@var_Birthdate", customer.Birthdate));

                    if (customer.Address is not null)
                    {
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
                    }

                    var sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read() == true)
                    {
                        try
                        {
                            Int32.TryParse(sqlDataReader["ReturnValue"].ToString(), out int parsedInt);
                            returnValue = parsedInt;
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
                    _sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                response.ResponseCode = 500;
                response.ResponseMessage = ex.ToString();
            }

            if (response.ResponseCode == null)
            {
                response.ResponseCode = 500;
                response.ResponseMessage = "Couldn't read ResponseCode";
            }

            return response;
        }

        public CustomerModel GetCustomer(GetCustomerRequest customer)
        {
            CustomerModel customerResponse = new CustomerModel();
            customerResponse.Address = new AddressModel();

            try
            {
                _sqlConnection.Open();

                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = _sqlConnection;
                    sqlCommand.CommandText = "dbo.usp_getCustomer";
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@var_SearchOption", customer.searchOption));
                    sqlCommand.Parameters.Add(new SqlParameter("@var_SearchVariable", customer.searchVariable));

                    var sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read() == true)
                    {
                        try
                        {
                            customerResponse.Guid = sqlDataReader["PK_customer_guid"].ToString();
                        }
                        catch { }
                        try
                        {
                            customerResponse.FirstName = sqlDataReader["first_name"].ToString();
                        }
                        catch { }
                        try
                        {
                            customerResponse.LastName = sqlDataReader["last_name"].ToString();
                        }
                        catch { }
                        try
                        {
                            customerResponse.Email = sqlDataReader["email"].ToString();
                        }
                        catch { }
                        try
                        {
                            customerResponse.MSISDN = sqlDataReader["msisdn"].ToString();
                        }
                        catch { }
                        try
                        {
                            Int32.TryParse(sqlDataReader["gender"].ToString(), out int parsedInt);
                            customerResponse.Gender = parsedInt;
                        }
                        catch { }
                        try
                        {
                            customerResponse.Birthdate = sqlDataReader["birthDate"].ToString();
                        }
                        catch { }
                        try
                        {
                            Int32.TryParse(sqlDataReader["customer_Status"].ToString(), out int parsedCustomerStatus);
                            customerResponse.CustomerStatus = parsedCustomerStatus;
                        }
                        catch { }
                        try
                        {
                            customerResponse.Address.Country = sqlDataReader["country"].ToString();
                        }
                        catch { }
                        try
                        {
                            customerResponse.Address.County = sqlDataReader["county"].ToString();
                        }
                        catch { }
                        try
                        {
                            customerResponse.Address.ZIP = sqlDataReader["zip_code"].ToString();
                        }
                        catch { }
                        try
                        {
                            customerResponse.Address.Town = sqlDataReader["town"].ToString();
                        }
                        catch { }
                        try
                        {
                            customerResponse.Address.Street = sqlDataReader["street"].ToString();
                        }
                        catch { }
                        try
                        {
                            customerResponse.Address.Number = sqlDataReader["number"].ToString();
                        }
                        catch { }
                    }
                    sqlDataReader.Close();
                    _sqlConnection.Close();
                    if (!String.IsNullOrEmpty(customerResponse.Guid))
                    {
                        customerResponse.ResponseCode = 200;
                        customerResponse.ResponseMessage = "Customer found in the DB!";
                    }
                    else
                    {
                        customerResponse.ResponseCode = 404;
                        customerResponse.ResponseMessage = "Customer was not found in the DB!";
                    }
                }
            }
            catch (Exception ex)
            {
                customerResponse.ResponseCode = 500;
                customerResponse.ResponseMessage = ex.ToString();
            }

            if (customerResponse.ResponseCode == null)
            {
                customerResponse.ResponseCode = 500;
                customerResponse.ResponseMessage = "Couldn't read ResponseCode";
            }

            return customerResponse;
        }

        public CustomerListModel GetCustomers()
        {
            CustomerListModel customerListResponse = new CustomerListModel();
            List<CustomerModel> customerList = new List<CustomerModel>();

            try
            {
                _sqlConnection.Open();

                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = _sqlConnection;
                    sqlCommand.CommandText = "dbo.usp_getCustomers";
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    var sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read() == true)
                    {
                        CustomerModel customer = new CustomerModel();
                        customer.Address = new AddressModel();

                        try
                        {
                            customer.Guid = sqlDataReader["PK_customer_guid"].ToString();
                        }
                        catch { }
                        try
                        {
                            customer.FirstName = sqlDataReader["first_name"].ToString();
                        }
                        catch { }
                        try
                        {
                            customer.LastName = sqlDataReader["last_name"].ToString();
                        }
                        catch { }
                        try
                        {
                            customer.Email = sqlDataReader["email"].ToString();
                        }
                        catch { }
                        try
                        {
                            customer.MSISDN = sqlDataReader["msisdn"].ToString();
                        }
                        catch { }
                        try
                        {
                            Int32.TryParse(sqlDataReader["gender"].ToString(), out int parsedGender);
                            customer.Gender = parsedGender;
                        }
                        catch { }
                        try
                        {
                            customer.Birthdate = sqlDataReader["birthDate"].ToString();
                        }
                        catch { }
                        try
                        {
                            Int32.TryParse(sqlDataReader["customer_Status"].ToString(), out int parsedCustomerStatus);
                            customer.CustomerStatus = parsedCustomerStatus;
                        }
                        catch { }
                        try
                        {
                            customer.Address.Country = sqlDataReader["country"].ToString();
                        }
                        catch { }
                        try
                        {
                            customer.Address.County = sqlDataReader["county"].ToString();
                        }
                        catch { }
                        try
                        {
                            customer.Address.ZIP = sqlDataReader["zip_code"].ToString();
                        }
                        catch { }
                        try
                        {
                            customer.Address.Town = sqlDataReader["town"].ToString();
                        }
                        catch { }
                        try
                        {
                            customer.Address.Street = sqlDataReader["street"].ToString();
                        }
                        catch { }
                        try
                        {
                            customer.Address.Number = sqlDataReader["number"].ToString();
                        }
                        catch { }

                        customerList.Add(customer);
                    }
                    sqlDataReader.Close();
                    _sqlConnection.Close();
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

            if (customerListResponse.ResponseCode == null)
            {
                customerListResponse.ResponseCode = 500;
                customerListResponse.ResponseMessage = "Couldn't read ResponseCode";
            }

            return customerListResponse;
        }

        public ResponseModel EditCustomer(CustomerModel customer)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                _sqlConnection.Open();

                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = _sqlConnection;
                    sqlCommand.CommandText = "dbo.usp_editCustomer";
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    sqlCommand.Parameters.Add(new SqlParameter("@var_Guid", customer.Guid));
                    sqlCommand.Parameters.Add(new SqlParameter("@var_FirstName", customer.FirstName));
                    sqlCommand.Parameters.Add(new SqlParameter("@var_LastName", customer.LastName));
                    sqlCommand.Parameters.Add(new SqlParameter("@var_Email", customer.Email));
                    sqlCommand.Parameters.Add(new SqlParameter("@var_MSISDN", customer.MSISDN));
                    sqlCommand.Parameters.Add(new SqlParameter("@var_Gender", customer.Gender));
                    sqlCommand.Parameters.Add(new SqlParameter("@var_Birthdate", customer.Birthdate));

                    if (customer.Address is not null)
                    {
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
                    }
                    var sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read() == true)
                    {
                        try
                        {
                            Int32.TryParse(sqlDataReader["customer_Status"].ToString(), out int customerStatusInt);
                            if (customerStatusInt == 1901)
                            {
                                response.ResponseCode = 200;
                                response.ResponseMessage = "Customer edited successfully!";
                            }
                            else if (customerStatusInt == 1903)
                            {
                                response.ResponseCode = 200;
                                response.ResponseMessage = "Customer edited successfully!";
                            }
                            else
                            {
                                response.ResponseCode = 500;
                                response.ResponseMessage = "Could not read customer status code!";
                            }
                        }
                        catch (Exception ex)
                        {
                            response.ResponseCode = 500;
                            response.ResponseMessage = ex.ToString();
                        }
                    }
                    sqlDataReader.Close();
                    _sqlConnection.Close();
                    _sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                response.ResponseCode = 500;
                response.ResponseMessage = ex.ToString();
            }

            if (response.ResponseCode == null)
            {
                response.ResponseCode = 500;
                response.ResponseMessage = "Couldn't read ResponseCode";
            }

            return response;
        }

        public ResponseModel DeactivateCustomer(string customerGUID)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                _sqlConnection.Open();

                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = _sqlConnection;
                    sqlCommand.CommandText = "dbo.usp_deactivateCustomer";
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    sqlCommand.Parameters.Add(new SqlParameter("@var_Guid", customerGUID));

                    var sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read() == true)
                    {
                        try
                        {
                            Int32.TryParse(sqlDataReader["customer_Status"].ToString(), out int customerStatusInt);
                            if (customerStatusInt == 1903)
                            {
                                response.ResponseCode = 200;
                                response.ResponseMessage = "Customer deactivated successfully!";
                            }
                            else
                            {
                                response.ResponseCode = 500;
                                response.ResponseMessage = "Could not read customer status code!";
                            }
                        }
                        catch (Exception ex)
                        {
                            response.ResponseCode = 500;
                            response.ResponseMessage = ex.ToString();
                        }
                    }
                    sqlDataReader.Close();
                    _sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                response.ResponseCode = 500;
                response.ResponseMessage = ex.ToString();
            }

            if (response.ResponseCode == null)
            {
                response.ResponseCode = 500;
                response.ResponseMessage = "Couldn't read ResponseCode";
            }

            return response;
        }

        public ResponseModel DeleteCustomer(string customerGUID)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                _sqlConnection.Open();

                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = _sqlConnection;
                    sqlCommand.CommandText = "dbo.usp_deleteCustomer";
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@var_Guid", customerGUID));

                    var sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read() == true)
                    {
                        try
                        {
                            Int32.TryParse(sqlDataReader["customer_Status"].ToString(), out int customerStatusInt);
                            if (customerStatusInt == 1903)
                            {
                                response.ResponseCode = 200;
                                response.ResponseMessage = "Customer deactivated successfully!";
                            }
                            else
                            {
                                response.ResponseCode = 500;
                                response.ResponseMessage = "Could not read customer status code!";
                            }
                        }
                        catch (Exception ex)
                        {
                            response.ResponseCode = 500;
                            response.ResponseMessage = ex.ToString();
                        }
                    }
                    sqlDataReader.Close();
                    _sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                response.ResponseCode = 500;
                response.ResponseMessage = ex.ToString();
            }

            if (response.ResponseCode == null)
            {
                response.ResponseCode = 500;
                response.ResponseMessage = "Couldn't read ResponseCode";
            }

            return response;
        }

        public bool CheckMerchantCredentialsFromDB(MerchantCredentials merchantCredentials)
        {
            bool isValid = false;

            try
            {
                _sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = _sqlConnection;
                    sqlCommand.CommandText = "dbo.usp_checkMerchantCredentials";
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@var_MerchantID", merchantCredentials.merchantID));
                    sqlCommand.Parameters.Add(new SqlParameter("@var_MerchantPassword", merchantCredentials.merchantPassword));

                    var sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read() == true)
                    {
                        try
                        {
                            if (sqlDataReader["merchant_role"] is null)
                            {
                            }
                            Int32.TryParse(sqlDataReader["merchant_role"].ToString(), out int merchantRoleInt);
                            if (merchantRoleInt == 1801)
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
                    _sqlConnection.Close();
                }
            }
            catch (Exception)
            {
                isValid = false;
            }

            return isValid;
        }
    }
}