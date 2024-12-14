using System.Data;
using CustomerManagementSystem.DataAccess.Configuration;
using CustomerManagementSystem.Domain.Models;
using Microsoft.Data.SqlClient;

namespace CustomerManagementSystem.DataAccess.DBConnection;

public class DbUtils : IDbUtils
{
    public DbUtils(IDalConfig configuration)
    {
        var currentSqlConnection = new CurrentSqlConnection(configuration);

            CurrentConnectionString = currentSqlConnection.GetCorrectSqlConnectionString();
    }
    
    private string CurrentConnectionString { get; set; }

    public ResponseModel RegisterCustomer(CustomerModel customer)
    {
        var response = new ResponseModel();
        var returnValue = 0;

        try
        {
            
            using (SqlConnection sqlConnection = new SqlConnection(CurrentConnectionString))
            {
                sqlConnection.Open();

                using (var sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandText = "dbo.usp_createCustomer";
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    sqlCommand.Parameters.Add(new SqlParameter("@var_Guid", Guid.NewGuid().ToString()));
                    sqlCommand.Parameters.Add(new SqlParameter("@var_FirstName", customer.FirstName));
                    sqlCommand.Parameters.Add(new SqlParameter("@var_LastName", customer.LastName));
                    sqlCommand.Parameters.Add(new SqlParameter("@var_Email", customer.Email));
                    sqlCommand.Parameters.Add(new SqlParameter("@var_MSISDN", customer.Msisdn));
                    sqlCommand.Parameters.Add(new SqlParameter("@var_Gender", customer.Gender));
                    sqlCommand.Parameters.Add(new SqlParameter("@var_Birthdate", customer.Birthdate));

                    if (customer.Address is not null)
                    {
                        var address = customer.Address;
                        if (address is not null)
                        {
                            sqlCommand.Parameters.Add(new SqlParameter("@var_Country", address.Country));
                            sqlCommand.Parameters.Add(new SqlParameter("@var_County", address.County));
                            sqlCommand.Parameters.Add(new SqlParameter("@var_Town", address.Town));
                            sqlCommand.Parameters.Add(new SqlParameter("@var_ZIP", address.Zip));
                            sqlCommand.Parameters.Add(new SqlParameter("@var_Street", address.Street));
                            sqlCommand.Parameters.Add(new SqlParameter("@var_Number", address.Number));
                        }
                    }

                    var sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        try
                        {
                            int.TryParse(sqlDataReader["ReturnValue"].ToString(), out var parsedInt);
                            returnValue = parsedInt;
                        }
                        catch
                        {
                        }

                        if (returnValue != 0 && returnValue != 200)
                        {
                            response.ResponseCode = 409;
                        }
                        else if (returnValue == 200)
                        {
                            response.ResponseCode = returnValue;
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
        var customerResponse = new CustomerModel();
        customerResponse.Address = new AddressModel();

        try
        {
            using (SqlConnection sqlConnection = new SqlConnection(CurrentConnectionString))
            {
                sqlConnection.Open();

                using (var sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandText = "dbo.usp_getCustomer";
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@var_SearchOption", customer.SearchOption));
                    sqlCommand.Parameters.Add(new SqlParameter("@var_SearchVariable", customer.SearchVariable));

                    var sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        try
                        {
                            customerResponse.Guid = sqlDataReader["PK_customer_guid"].ToString();
                        }
                        catch
                        {
                            // ignored
                        }

                        try
                        {
                            customerResponse.FirstName = sqlDataReader["first_name"].ToString();
                        }
                        catch
                        {
                            // ignored
                        }

                        try
                        {
                            customerResponse.LastName = sqlDataReader["last_name"].ToString();
                        }
                        catch
                        {
                            // ignored
                        }

                        try
                        {
                            customerResponse.Email = sqlDataReader["email"].ToString();
                        }
                        catch
                        {
                            // ignored
                        }

                        try
                        {
                            customerResponse.Msisdn = sqlDataReader["msisdn"].ToString();
                        }
                        catch
                        {
                            // ignored
                        }

                        try
                        {
                            int.TryParse(sqlDataReader["gender"].ToString(), out var parsedInt);
                            customerResponse.Gender = parsedInt;
                        }
                        catch
                        {
                            // ignored
                        }

                        try
                        {
                            customerResponse.Birthdate = sqlDataReader["birthDate"].ToString();
                        }
                        catch
                        {
                            // ignored
                        }

                        try
                        {
                            int.TryParse(sqlDataReader["customer_Status"].ToString(), out var parsedCustomerStatus);
                            customerResponse.CustomerStatus = parsedCustomerStatus;
                        }
                        catch
                        {
                            // ignored
                        }

                        try
                        {
                            customerResponse.Address.Country = sqlDataReader["country"].ToString();
                        }
                        catch
                        {
                            // ignored
                        }

                        try
                        {
                            customerResponse.Address.County = sqlDataReader["county"].ToString();
                        }
                        catch
                        {
                            // ignored
                        }

                        try
                        {
                            customerResponse.Address.Zip = sqlDataReader["zip_code"].ToString();
                        }
                        catch
                        {
                            // ignored
                        }

                        try
                        {
                            customerResponse.Address.Town = sqlDataReader["town"].ToString();
                        }
                        catch
                        {
                            // ignored
                        }

                        try
                        {
                            customerResponse.Address.Street = sqlDataReader["street"].ToString();
                        }
                        catch
                        {
                            // ignored
                        }

                        try
                        {
                            customerResponse.Address.Number = sqlDataReader["number"].ToString();
                        }
                        catch
                        {
                            // ignored
                        }
                    }

                    sqlDataReader.Close();
                    sqlConnection.Close();
                    if (!string.IsNullOrEmpty(customerResponse.Guid))
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
        var customerListResponse = new CustomerListModel();
        var customerList = new List<CustomerModel>();

        try
        {
            using (SqlConnection sqlConnection = new SqlConnection(CurrentConnectionString))
            {
                sqlConnection.Open();

                using (var sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandText = "dbo.usp_getCustomers";
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    var sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        var customer = new CustomerModel();
                        customer.Address = new AddressModel();

                        try
                        {
                            customer.Guid = sqlDataReader["PK_customer_guid"].ToString();
                        }
                        catch
                        {
                            // ignored
                        }

                        try
                        {
                            customer.FirstName = sqlDataReader["first_name"].ToString();
                        }
                        catch
                        {
                            // ignored
                        }

                        try
                        {
                            customer.LastName = sqlDataReader["last_name"].ToString();
                        }
                        catch
                        {
                            // ignored
                        }

                        try
                        {
                            customer.Email = sqlDataReader["email"].ToString();
                        }
                        catch
                        {
                            // ignored
                        }

                        try
                        {
                            customer.Msisdn = sqlDataReader["msisdn"].ToString();
                        }
                        catch
                        {
                            // ignored
                        }

                        try
                        {
                            int.TryParse(sqlDataReader["gender"].ToString(), out var parsedGender);
                            customer.Gender = parsedGender;
                        }
                        catch
                        {
                            // ignored
                        }

                        try
                        {
                            customer.Birthdate = sqlDataReader["birthDate"].ToString();
                        }
                        catch
                        {
                            // ignored
                        }

                        try
                        {
                            int.TryParse(sqlDataReader["customer_Status"].ToString(), out var parsedCustomerStatus);
                            customer.CustomerStatus = parsedCustomerStatus;
                        }
                        catch
                        {
                            // ignored
                        }

                        try
                        {
                            customer.Address.Country = sqlDataReader["country"].ToString();
                        }
                        catch
                        {
                            // ignored
                        }

                        try
                        {
                            customer.Address.County = sqlDataReader["county"].ToString();
                        }
                        catch
                        {
                            // ignored
                        }

                        try
                        {
                            customer.Address.Zip = sqlDataReader["zip_code"].ToString();
                        }
                        catch
                        {
                            // ignored
                        }

                        try
                        {
                            customer.Address.Town = sqlDataReader["town"].ToString();
                        }
                        catch
                        {
                            // ignored
                        }

                        try
                        {
                            customer.Address.Street = sqlDataReader["street"].ToString();
                        }
                        catch
                        {
                        }

                        try
                        {
                            customer.Address.Number = sqlDataReader["number"].ToString();
                        }
                        catch
                        {
                            // ignored
                        }

                        customerList.Add(customer);
                    }

                    sqlDataReader.Close();
                    sqlConnection.Close();
                    customerListResponse.ResponseCode = 200;
                    customerListResponse.ResponseMessage = "Customers found in DB!";
                    customerListResponse.CustomerList = customerList;
                }
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
        var response = new ResponseModel();

        try
        {
            using (SqlConnection sqlConnection = new SqlConnection(CurrentConnectionString))
            {
                sqlConnection.Open();

                using (var sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandText = "dbo.usp_editCustomer";
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    sqlCommand.Parameters.Add(new SqlParameter("@var_Guid", customer.Guid));
                    sqlCommand.Parameters.Add(new SqlParameter("@var_FirstName", customer.FirstName));
                    sqlCommand.Parameters.Add(new SqlParameter("@var_LastName", customer.LastName));
                    sqlCommand.Parameters.Add(new SqlParameter("@var_Email", customer.Email));
                    sqlCommand.Parameters.Add(new SqlParameter("@var_MSISDN", customer.Msisdn));
                    sqlCommand.Parameters.Add(new SqlParameter("@var_Gender", customer.Gender));
                    sqlCommand.Parameters.Add(new SqlParameter("@var_Birthdate", customer.Birthdate));

                    if (customer.Address is not null)
                    {
                        var address = customer.Address;
                        if (address is not null)
                        {
                            sqlCommand.Parameters.Add(new SqlParameter("@var_Country", address.Country));
                            sqlCommand.Parameters.Add(new SqlParameter("@var_County", address.County));
                            sqlCommand.Parameters.Add(new SqlParameter("@var_Town", address.Town));
                            sqlCommand.Parameters.Add(new SqlParameter("@var_ZIP", address.Zip));
                            sqlCommand.Parameters.Add(new SqlParameter("@var_Street", address.Street));
                            sqlCommand.Parameters.Add(new SqlParameter("@var_Number", address.Number));
                        }
                    }

                    var sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                        try
                        {
                            int.TryParse(sqlDataReader["customer_Status"].ToString(), out var customerStatusInt);
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

                    sqlDataReader.Close();
                    sqlConnection.Close();
                }
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

    public ResponseModel DeactivateCustomer(string customerGuid)
    {
        var response = new ResponseModel();

        try
        {
            using (SqlConnection sqlConnection = new SqlConnection(CurrentConnectionString))
            {
                sqlConnection.Open();

                using (var sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandText = "dbo.usp_deactivateCustomer";
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    sqlCommand.Parameters.Add(new SqlParameter("@var_Guid", customerGuid));

                    var sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                        try
                        {
                            int.TryParse(sqlDataReader["customer_Status"].ToString(), out var customerStatusInt);
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

                    sqlDataReader.Close();
                    sqlConnection.Close();
                }
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
        var response = new ResponseModel();

        try
        {
            using (SqlConnection sqlConnection = new SqlConnection(CurrentConnectionString))
            {
                sqlConnection.Open();

                using (var sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandText = "dbo.usp_deleteCustomer";
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@var_Guid", customerGUID));

                    var sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                        try
                        {
                            int.TryParse(sqlDataReader["customer_Status"].ToString(), out var customerStatusInt);
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

                    sqlDataReader.Close();
                    sqlConnection.Close();
                }
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

    public async Task<MerchantCredentialsCheck> CheckMerchantCredentialsFromDb(MerchantCredentials merchantCredentials)
    {
        var merchantCredentialsCheck = new MerchantCredentialsCheck
        {
            IsValid = false,
            ErrorMessage = null,
        };

        try
        {
            await using var sqlConnection = new SqlConnection(CurrentConnectionString);
            await sqlConnection.OpenAsync();

            const string storedProcedure = "dbo.usp_checkMerchantCredentials";

            await using var sqlCommand = new SqlCommand(storedProcedure, sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            sqlCommand.Parameters.AddWithValue("@var_MerchantID", merchantCredentials.MerchantId);
            sqlCommand.Parameters.AddWithValue("@var_MerchantPassword", merchantCredentials.MerchantPassword);

            await using var reader = await sqlCommand.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                merchantCredentialsCheck.IsValid = int.TryParse(reader["merchant_role"].ToString(), out var merchantRoleInt) && merchantRoleInt == 1801;

                if (!merchantCredentialsCheck.IsValid)
                {
                    merchantCredentialsCheck.ErrorMessage = "The provided merchant credentials were invalid!";
                }
            }
        }
        catch (Exception ex)
        {
            merchantCredentialsCheck.IsValid = false;
            merchantCredentialsCheck.ErrorMessage = ex.Message;
        }

        return merchantCredentialsCheck;
    }

}