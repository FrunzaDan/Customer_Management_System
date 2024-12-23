using System.Data;
using CustomerManagementSystem.DataAccess.Configuration;
using CustomerManagementSystem.Domain.Models;
using Microsoft.Data.SqlClient;

namespace CustomerManagementSystem.DataAccess.DBConnection;



public class DbUtils(IDalConfig configuration) : IDbUtils
{
    private string? CurrentConnectionString { get; set; }

    public async Task<ResponseModel<object>> RegisterCustomer(CustomerModel customer)
    {
        const string storedProcedure = "dbo.usp_createCustomer";
        return await ExecuteStoredProcedureAsync<ResponseModel<object>>(storedProcedure,
            command =>
            {
                DbHelper.AddCustomerParameters(command, customer);
            },
            async reader =>
            {
                if (!await reader.ReadAsync().ConfigureAwait(false))
                    return new ResponseModel<object>
                    {
                        Status = 500,
                        ResponseMessage = "No data returned or failed to process request."
                    };
                if (reader["ReturnValue"] is int returnValue)
                    return returnValue switch
                    {
                        200 => new ResponseModel<object>
                        {
                            Status = 200, ResponseMessage = "Customer created successfully!"
                        },
                        4001 => new ResponseModel<object> { Status = 400, ResponseMessage = "MSISDN already exists." },
                        4002 => new ResponseModel<object> { Status = 400, ResponseMessage = "Email already exists." },
                        _ => new ResponseModel<object> { Status = 500, ResponseMessage = "Internal error occurred." }
                    };

                return new ResponseModel<object>
                {
                    Status = 500,
                    ResponseMessage = "Failed to create customer."
                };

            });
    }

    public async Task<ResponseModel<CustomerModel>> GetCustomer(GetCustomerRequest request)
    {
        const string storedProcedure = "dbo.usp_getCustomer";
        return await ExecuteStoredProcedureAsync<ResponseModel<CustomerModel>>(storedProcedure, command =>
            {
                command.Parameters.AddWithValue("@var_SearchOption", request.SearchOption);
                command.Parameters.AddWithValue("@var_SearchVariable", request.SearchVariable ?? (object)DBNull.Value);
            },
            async reader => await reader.ReadAsync().ConfigureAwait(false)
                ? new ResponseModel<CustomerModel>
                {
                    Status = 200,
                    ResponseMessage = "Customer found.",
                    Data = DbHelper.MapCustomerFromReader(reader)
                }
                : new ResponseModel<CustomerModel>
                {
                    Status = 404,
                    ResponseMessage = "Customer not found",
                    Data = null
                });
    }

    public async Task<ResponseModel<CustomerListModel>> GetCustomers()
    {
        const string storedProcedure = "dbo.usp_getCustomers";
        return await ExecuteStoredProcedureAsync<ResponseModel<CustomerListModel>>(storedProcedure, null, async reader =>
        {
            var customers = new List<CustomerModel>();
            while (await reader.ReadAsync().ConfigureAwait(false))
                customers.Add(DbHelper.MapCustomerFromReader(reader));

            return new ResponseModel<CustomerListModel>
            {
                Status = 200,
                ResponseMessage = $"{customers.Count} customers found.",
                Data = new CustomerListModel { CustomerList = customers }
            };
        });
    }

    public async Task<ResponseModel<object>> EditCustomer(CustomerModel customer)
    {
        const string storedProcedure = "dbo.usp_editCustomer";
        return await ExecuteStoredProcedureAsync<ResponseModel<object>>(storedProcedure,
            command => { DbHelper.AddCustomerParameters(command, customer); },
            async reader =>
            {
                if (!await reader.ReadAsync().ConfigureAwait(false))
                    return new ResponseModel<object>
                    {
                        Status = 500,
                        ResponseMessage = "No data returned or customer update failed."
                    };
                var message = reader["message"] as string;

                if (reader["result"] is 1)
                    return new ResponseModel<object>
                    {
                        Status = 200,
                        ResponseMessage = message ?? "Customer details updated successfully!"
                    };

                return new ResponseModel<object>
                {
                    Status = 500,
                    ResponseMessage = message ?? "Failed to update customer."
                };

            });
    }

    public async Task<ResponseModel<object>> DeactivateCustomer(string customerGuid)
    {
        const string storedProcedure = "dbo.usp_deactivateCustomer";
        return await ExecuteStoredProcedureAsync<ResponseModel<object>>(storedProcedure,
            command => { command.Parameters.AddWithValue("@var_Guid", customerGuid); }, async reader =>
            {
                if (!await reader.ReadAsync().ConfigureAwait(false))
                    return new ResponseModel<object>
                    {
                        Status = 500,
                        ResponseMessage = "Customer not found or no data returned from the procedure."
                    };
                var message = reader["message"] as string;

                if (reader["result"] is 1)
                    return new ResponseModel<object>
                    {
                        Status = 200,
                        ResponseMessage = message ?? "Customer deactivated successfully!"
                    };

                return new ResponseModel<object>
                {
                    Status = 500,
                    ResponseMessage = message ?? "Failed to deactivate customer!"
                };

            });
    }

    public async Task<ResponseModel<object>> DeleteCustomer(string customerGuid)
    {
        const string storedProcedure = "dbo.usp_deleteCustomer";
        return await ExecuteStoredProcedureAsync<ResponseModel<object>>(storedProcedure,
            command => { command.Parameters.AddWithValue("@var_Guid", customerGuid); }, async reader =>
            {
                if (!await reader.ReadAsync().ConfigureAwait(false))
                    return new ResponseModel<object>
                    {
                        Status = 500,
                        ResponseMessage = "Customer not found or no data returned from the procedure."
                    };
                var message = reader["message"] as string;

                if (reader["result"] is 1)
                    return new ResponseModel<object>
                    {
                        Status = 200,
                        ResponseMessage = message ?? "Customer deleted successfully!"
                    };

                return new ResponseModel<object>
                {
                    Status = 500,
                    ResponseMessage = message ?? "Failed to delete customer."
                };

            });
    }

    public async Task<ResponseModel<ResultValidityCheck>> CheckMerchantCredentialsFromDb(MerchantCredentials merchantCredentials)
    {
        const string storedProcedure = "dbo.usp_checkMerchantCredentials";
        return await ExecuteStoredProcedureAsync<ResponseModel<ResultValidityCheck>>(storedProcedure, command =>
        {
            command.Parameters.AddWithValue("@var_MerchantID", merchantCredentials.MerchantId);
            command.Parameters.AddWithValue("@var_MerchantPassword", merchantCredentials.MerchantPassword);
        }, async reader =>
        {
            var result = new ResultValidityCheck { IsValid = false };

            if (!await reader.ReadAsync().ConfigureAwait(false))
                return new ResponseModel<ResultValidityCheck>
                {
                    Status = 404,
                    ResponseMessage = "Invalid merchant credentials or no matching merchant found!",
                    Data = result
                };

            if (reader["merchant_role"] is int role)
            {
                result.IsValid = (int?)role == 1801;
                if (!result.IsValid) result.ErrorMessage = "The provided merchant credentials have invalid roles!";
            }
            else
            {
                result.ErrorMessage = "Invalid merchant credentials or no matching merchant found!";
            }

            return new ResponseModel<ResultValidityCheck>
            {
                Status = result.IsValid ? 200 : 400,
                ResponseMessage = result.IsValid ? "Valid merchant credentials." : result.ErrorMessage,
                Data = result
            };
        });
    }

    private void CheckConnectionString()
    {
        if (!string.IsNullOrEmpty(CurrentConnectionString)) return;
        var currentSqlConnection = new CurrentSqlConnection(configuration);
        CurrentConnectionString = currentSqlConnection.GetCorrectSqlConnectionString();
    }

    private async Task<T> ExecuteStoredProcedureAsync<T>(
        string storedProcedure,
        Action<SqlCommand>? configureCommand,
        Func<SqlDataReader, Task<T>> handleReader)
    {
        CheckConnectionString();
        await using var connection = new SqlConnection(CurrentConnectionString);
        await connection.OpenAsync().ConfigureAwait(false);
        await using var command = new SqlCommand(storedProcedure, connection);
        command.CommandType = CommandType.StoredProcedure;

        // If the configureCommand delegate is provided, invoke it to allow for custom configuration of the SqlCommand
        configureCommand?.Invoke(command);

        await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

        // Pass the SqlDataReader to the handleReader function to process the data and return a result of type T.
        return await handleReader(reader);
    }
}
