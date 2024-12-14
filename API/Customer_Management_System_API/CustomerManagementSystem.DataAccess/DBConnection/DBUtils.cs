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
                // Add address parameters if present
                if (customer.Address is not null)
                {
                    sqlCommand.Parameters.AddWithValue("@var_Country",
                        customer.Address.Country ?? (object)DBNull.Value);
                    sqlCommand.Parameters.AddWithValue("@var_County", customer.Address.County ?? (object)DBNull.Value);
                    sqlCommand.Parameters.AddWithValue("@var_Town", customer.Address.Town ?? (object)DBNull.Value);
                    sqlCommand.Parameters.AddWithValue("@var_ZIP", customer.Address.Zip ?? (object)DBNull.Value);
                    sqlCommand.Parameters.AddWithValue("@var_Street", customer.Address.Street ?? (object)DBNull.Value);
                    sqlCommand.Parameters.AddWithValue("@var_Number", customer.Address.Number ?? (object)DBNull.Value);
                }

            await using var reader = await sqlCommand.ExecuteReaderAsync();

            if (await reader.ReadAsync() && int.TryParse(reader["ReturnValue"].ToString(), out var parsedReturnValue))
            {
                response.Status = parsedReturnValue switch
                {
                    200 => 200,
                    _ => 409
                };
                response.ResponseMessage = parsedReturnValue == 200 ? "Success" : "Conflict occurred.";
            }
            else
            {
                response.Status = 500;
                response.ResponseMessage = "Failed to connect to DB or retrieve a valid response.";
            }
        }
        catch (Exception ex)
        {
            response.Status = 500;
            response.ResponseMessage = ex.ToString();
        }

        if (response.Status != null) return response;
        response.Status = 500;
        response.ResponseMessage = "Couldn't read ResponseCode";

        return response;
    }

    public async Task<CustomerModel> GetCustomer(GetCustomerRequest request)
    {
        var customerResponse = new CustomerModel { Address = new AddressModel() };

        try
        {
            await using var sqlConnection = new SqlConnection(CurrentConnectionString);
            await sqlConnection.OpenAsync();

            const string storedProcedure = "dbo.usp_getCustomer";

            await using var sqlCommand = new SqlCommand(storedProcedure, sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            // Use explicit parameterization for clarity
            sqlCommand.Parameters.Add(new SqlParameter("@var_SearchOption", SqlDbType.NVarChar)
                { Value = request.SearchOption });
            sqlCommand.Parameters.Add(new SqlParameter("@var_SearchVariable", SqlDbType.NVarChar)
                { Value = request.SearchVariable ?? (object)DBNull.Value });

            await using var reader = await sqlCommand.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                customerResponse = DbHelper.MapCustomerFromReader(reader);
                customerResponse.Status = 200;
                customerResponse.ResponseMessage = "Customer found in the DB!";
            }
            else
            {
                customerResponse.Status = 404;
                customerResponse.ResponseMessage = "Customer was not found in the DB!";
            }
        }
        catch (Exception ex)
        {
            customerResponse.Status = 500;
            customerResponse.ResponseMessage = ex.Message; // or ex.ToString() for more detail
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

            while (await reader.ReadAsync()) customerList.Add(DbHelper.MapCustomerFromReader(reader));

            customerListResponse.Status = 200;
            customerListResponse.ResponseMessage = $"{customerList.Count} customers found in DB!";
            customerListResponse.CustomerList = customerList;
        }
        catch (Exception ex)
        {
            customerListResponse.Status = 500;
            customerListResponse.ResponseMessage = ex.Message;
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
                        response.Status = 200;
                        response.ResponseMessage = "Customer edited successfully!";
                    }
                    else if (customerStatusInt == 1903)
                    {
                        response.Status = 200;
                        response.ResponseMessage = "Customer edited successfully!";
                    }
                    else
                    {
                        response.Status = 500;
                        response.ResponseMessage = "Could not read customer status code!";
                    }
                }
                catch (Exception ex)
                {
                    response.Status = 500;
                    response.ResponseMessage = ex.ToString();
                }
        }
        catch (Exception ex)
        {
            response.Status = 500;
            response.ResponseMessage = ex.ToString();
        }

        if (response.Status == null)
        {
            response.Status = 500;
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
                        response.Status = 200;
                        response.ResponseMessage = "Customer deactivated successfully!";
                    }
                    else
                    {
                        response.Status = 500;
                        response.ResponseMessage = "Could not read customer status code!";
                    }
                }
                catch (Exception ex)
                {
                    response.Status = 500;
                    response.ResponseMessage = ex.ToString();
                }
        }
        catch (Exception ex)
        {
            response.Status = 500;
            response.ResponseMessage = ex.ToString();
        }

        if (response.Status == null)
        {
            response.Status = 500;
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
                        response.Status = 200;
                        response.ResponseMessage = "Customer deactivated successfully!";
                    }
                    else
                    {
                        response.Status = 500;
                        response.ResponseMessage = "Could not read customer status code!";
                    }
                }
                catch (Exception ex)
                {
                    response.Status = 500;
                    response.ResponseMessage = ex.ToString();
                }
        }
        catch (Exception ex)
        {
            response.Status = 500;
            response.ResponseMessage = ex.ToString();
        }

        if (response.Status == null)
        {
            response.Status = 500;
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