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

    private string CurrentConnectionString { get; }

    public async Task<ResponseModel> RegisterCustomer(CustomerModel customer)
    {
        var response = new ResponseModel();
        var returnValue = 0;

        try
        {
            await using var sqlConnection = new SqlConnection(CurrentConnectionString);
            await sqlConnection.OpenAsync();

            const string storedProcedure = "dbo.usp_createCustomer";

            await using var sqlCommand = new SqlCommand(storedProcedure, sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            sqlCommand.Parameters.AddWithValue("@var_Guid", Guid.NewGuid().ToString());
            sqlCommand.Parameters.AddWithValue("@var_FirstName", customer.FirstName);
            sqlCommand.Parameters.AddWithValue("@var_LastName", customer.LastName);
            sqlCommand.Parameters.AddWithValue("@var_Email", customer.Email);
            sqlCommand.Parameters.AddWithValue("@var_MSISDN", customer.Msisdn);
            sqlCommand.Parameters.AddWithValue("@var_Gender", customer.Gender);
            sqlCommand.Parameters.AddWithValue("@var_Birthdate", customer.Birthdate);

            if (customer.Address is not null)
            {
                var address = customer.Address;
                if (address is not null)
                {
                    sqlCommand.Parameters.AddWithValue("@var_Country", address.Country);
                    sqlCommand.Parameters.AddWithValue("@var_County", address.County);
                    sqlCommand.Parameters.AddWithValue("@var_Town", address.Town);
                    sqlCommand.Parameters.AddWithValue("@var_ZIP", address.Zip);
                    sqlCommand.Parameters.AddWithValue("@var_Street", address.Street);
                    sqlCommand.Parameters.AddWithValue("@var_Number", address.Number);
                }
            }

            await using var reader = await sqlCommand.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                try
                {
                    int.TryParse(reader["ReturnValue"].ToString(), out var parsedInt);
                    returnValue = parsedInt;
                }
                catch
                {
                    // ignored
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
        }
        catch (Exception ex)
        {
            response.ResponseCode = 500;
            response.ResponseMessage = ex.ToString();
        }

        if (response.ResponseCode != null) return response;
        response.ResponseCode = 500;
        response.ResponseMessage = "Couldn't read ResponseCode";

        return response;
    }

    public async Task<CustomerModel> GetCustomer(GetCustomerRequest customer)
    {
        var customerResponse = new CustomerModel();
        customerResponse.Address = new AddressModel();

        try
        {
            await using var sqlConnection = new SqlConnection(CurrentConnectionString);
            await sqlConnection.OpenAsync();

            const string storedProcedure = "dbo.usp_getCustomer";

            await using var sqlCommand = new SqlCommand(storedProcedure, sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@var_SearchOption", customer.SearchOption);
            sqlCommand.Parameters.AddWithValue("@var_SearchVariable", customer.SearchVariable);

            await using var reader = await sqlCommand.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                try
                {
                    customerResponse.Guid = reader["PK_customer_guid"].ToString();
                }
                catch
                {
                    // ignored
                }

                try
                {
                    customerResponse.FirstName = reader["first_name"].ToString();
                }
                catch
                {
                    // ignored
                }

                try
                {
                    customerResponse.LastName = reader["last_name"].ToString();
                }
                catch
                {
                    // ignored
                }

                try
                {
                    customerResponse.Email = reader["email"].ToString();
                }
                catch
                {
                    // ignored
                }

                try
                {
                    customerResponse.Msisdn = reader["msisdn"].ToString();
                }
                catch
                {
                    // ignored
                }

                try
                {
                    int.TryParse(reader["gender"].ToString(), out var parsedInt);
                    customerResponse.Gender = parsedInt;
                }
                catch
                {
                    // ignored
                }

                try
                {
                    customerResponse.Birthdate = reader["birthDate"].ToString();
                }
                catch
                {
                    // ignored
                }

                try
                {
                    int.TryParse(reader["customer_Status"].ToString(), out var parsedCustomerStatus);
                    customerResponse.CustomerStatus = parsedCustomerStatus;
                }
                catch
                {
                    // ignored
                }

                try
                {
                    customerResponse.Address.Country = reader["country"].ToString();
                }
                catch
                {
                    // ignored
                }

                try
                {
                    customerResponse.Address.County = reader["county"].ToString();
                }
                catch
                {
                    // ignored
                }

                try
                {
                    customerResponse.Address.Zip = reader["zip_code"].ToString();
                }
                catch
                {
                    // ignored
                }

                try
                {
                    customerResponse.Address.Town = reader["town"].ToString();
                }
                catch
                {
                    // ignored
                }

                try
                {
                    customerResponse.Address.Street = reader["street"].ToString();
                }
                catch
                {
                    // ignored
                }

                try
                {
                    customerResponse.Address.Number = reader["number"].ToString();
                }
                catch
                {
                    // ignored
                }
            }

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

    public async Task<CustomerListModel> GetCustomers()
    {
        var customerListResponse = new CustomerListModel();
        var customerList = new List<CustomerModel>();

        try
        {
            await using var sqlConnection = new SqlConnection(CurrentConnectionString);
            await sqlConnection.OpenAsync();

            const string storedProcedure = "dbo.usp_getCustomers";

            await using var sqlCommand = new SqlCommand(storedProcedure, sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            await using var reader = await sqlCommand.ExecuteReaderAsync();
            while (reader.Read())
            {
                var customer = new CustomerModel();
                customer.Address = new AddressModel();

                try
                {
                    customer.Guid = reader["PK_customer_guid"].ToString();
                }
                catch
                {
                    // ignored
                }

                try
                {
                    customer.FirstName = reader["first_name"].ToString();
                }
                catch
                {
                    // ignored
                }

                try
                {
                    customer.LastName = reader["last_name"].ToString();
                }
                catch
                {
                    // ignored
                }

                try
                {
                    customer.Email = reader["email"].ToString();
                }
                catch
                {
                    // ignored
                }

                try
                {
                    customer.Msisdn = reader["msisdn"].ToString();
                }
                catch
                {
                    // ignored
                }

                try
                {
                    int.TryParse(reader["gender"].ToString(), out var parsedGender);
                    customer.Gender = parsedGender;
                }
                catch
                {
                    // ignored
                }

                try
                {
                    customer.Birthdate = reader["birthDate"].ToString();
                }
                catch
                {
                    // ignored
                }

                try
                {
                    int.TryParse(reader["customer_Status"].ToString(), out var parsedCustomerStatus);
                    customer.CustomerStatus = parsedCustomerStatus;
                }
                catch
                {
                    // ignored
                }

                try
                {
                    customer.Address.Country = reader["country"].ToString();
                }
                catch
                {
                    // ignored
                }

                try
                {
                    customer.Address.County = reader["county"].ToString();
                }
                catch
                {
                    // ignored
                }

                try
                {
                    customer.Address.Zip = reader["zip_code"].ToString();
                }
                catch
                {
                    // ignored
                }

                try
                {
                    customer.Address.Town = reader["town"].ToString();
                }
                catch
                {
                    // ignored
                }

                try
                {
                    customer.Address.Street = reader["street"].ToString();
                }
                catch
                {
                    // ignored
                }

                try
                {
                    customer.Address.Number = reader["number"].ToString();
                }
                catch
                {
                    // ignored
                }

                customerList.Add(customer);
            }

            customerListResponse.ResponseCode = 200;
            customerListResponse.ResponseMessage = customerList.Count.ToString() + " customers found in DB!";
            customerListResponse.CustomerList = customerList;
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

    public async Task<ResponseModel> EditCustomer(CustomerModel customer)
    {
        var response = new ResponseModel();

        try
        {
            await using var sqlConnection = new SqlConnection(CurrentConnectionString);
            await sqlConnection.OpenAsync();

            const string storedProcedure = "dbo.usp_editCustomer";

            await using var sqlCommand = new SqlCommand(storedProcedure, sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            sqlCommand.Parameters.AddWithValue("@var_Guid", customer.Guid);
            sqlCommand.Parameters.AddWithValue("@var_FirstName", customer.FirstName);
            sqlCommand.Parameters.AddWithValue("@var_LastName", customer.LastName);
            sqlCommand.Parameters.AddWithValue("@var_Email", customer.Email);
            sqlCommand.Parameters.AddWithValue("@var_MSISDN", customer.Msisdn);
            sqlCommand.Parameters.AddWithValue("@var_Gender", customer.Gender);
            sqlCommand.Parameters.AddWithValue("@var_Birthdate", customer.Birthdate);

            if (customer.Address is not null)
            {
                var address = customer.Address;
                if (address is not null)
                {
                    sqlCommand.Parameters.AddWithValue("@var_Country", address.Country);
                    sqlCommand.Parameters.AddWithValue("@var_County", address.County);
                    sqlCommand.Parameters.AddWithValue("@var_Town", address.Town);
                    sqlCommand.Parameters.AddWithValue("@var_ZIP", address.Zip);
                    sqlCommand.Parameters.AddWithValue("@var_Street", address.Street);
                    sqlCommand.Parameters.AddWithValue("@var_Number", address.Number);
                }
            }

            await using var reader = await sqlCommand.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                try
                {
                    int.TryParse(reader["customer_Status"].ToString(), out var customerStatusInt);
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

    public async Task<ResponseModel> DeactivateCustomer(string customerGuid)
    {
        var response = new ResponseModel();

        try
        {
            await using var sqlConnection = new SqlConnection(CurrentConnectionString);
            await sqlConnection.OpenAsync();

            const string storedProcedure = "dbo.usp_deactivateCustomer";

            await using var sqlCommand = new SqlCommand(storedProcedure, sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            sqlCommand.Parameters.AddWithValue("@var_Guid", customerGuid);

            await using var reader = await sqlCommand.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                try
                {
                    int.TryParse(reader["customer_Status"].ToString(), out var customerStatusInt);
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

    public async Task<ResponseModel> DeleteCustomer(string customerGuid)
    {
        var response = new ResponseModel();

        try
        {
            await using var sqlConnection = new SqlConnection(CurrentConnectionString);
            await sqlConnection.OpenAsync();

            const string storedProcedure = "dbo.usp_deleteCustomer";

            await using var sqlCommand = new SqlCommand(storedProcedure, sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@var_Guid", customerGuid);

            await using var reader = await sqlCommand.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                try
                {
                    int.TryParse(reader["customer_Status"].ToString(), out var customerStatusInt);
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
            ErrorMessage = null
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
                merchantCredentialsCheck.IsValid =
                    int.TryParse(reader["merchant_role"].ToString(), out var merchantRoleInt) &&
                    merchantRoleInt == 1801;

                if (!merchantCredentialsCheck.IsValid)
                    merchantCredentialsCheck.ErrorMessage = "The provided merchant credentials were invalid!";
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