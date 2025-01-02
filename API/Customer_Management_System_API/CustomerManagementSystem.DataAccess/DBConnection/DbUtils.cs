using System.Data;
using CustomerManagementSystem.Domain.Configuration;
using CustomerManagementSystem.Domain.Models;
using Microsoft.Data.SqlClient;

namespace CustomerManagementSystem.DataAccess.DBConnection;

public class DbUtils(IAppSettingsConfig configuration) : IDbUtils
{
    private string? CurrentConnectionString { get; set; }

    public async Task<ResponseModel<object>> RegisterCustomer(CustomerModel customer)
    {
        return await ExecuteStoredProcedureAsync(
            "dbo.usp_createCustomer",
            command => DbHelper.AddCustomerParameters(command, customer),
            async reader => await HandleRegisterCustomerResponse(reader));
    }

    public async Task<ResponseModel<object>> GetCustomer(GetCustomerRequest request)
    {
        return await ExecuteStoredProcedureAsync(
            "dbo.usp_getCustomer",
            command =>
            {
                command.Parameters.AddWithValue("@var_SearchOption", request.SearchOption);
                command.Parameters.AddWithValue("@var_SearchVariable", request.SearchVariable ?? (object)DBNull.Value);
            },
            async reader => await HandleGetCustomerResponse(reader));
    }

    public async Task<ResponseModel<object>> GetCustomers()
    {
        return await ExecuteStoredProcedureAsync(
            "dbo.usp_getCustomers",
            null,
            async reader => await HandleGetCustomersResponse(reader));
    }

    public async Task<ResponseModel<object>> EditCustomer(CustomerModel customer)
    {
        return await ExecuteStoredProcedureAsync(
            "dbo.usp_editCustomer",
            command => DbHelper.AddCustomerParameters(command, customer),
            async reader => await HandleEditCustomerResponse(reader));
    }

    public async Task<ResponseModel<object>> DeactivateCustomer(string customerGuid)
    {
        return await ExecuteStoredProcedureAsync(
            "dbo.usp_deactivateCustomer",
            command => command.Parameters.AddWithValue("@var_Guid", customerGuid),
            async reader => await HandleDeactivateOrDeleteCustomerResponse(reader));
    }

    public async Task<ResponseModel<object>> DeleteCustomer(string customerGuid)
    {
        return await ExecuteStoredProcedureAsync(
            "dbo.usp_deleteCustomer",
            command => command.Parameters.AddWithValue("@var_Guid", customerGuid),
            async reader => await HandleDeactivateOrDeleteCustomerResponse(reader));
    }

    public async Task<ResponseModel<object>> CheckMerchantCredentialsFromDb(MerchantCredentials merchantCredentials)
    {
        return await ExecuteStoredProcedureAsync(
            "dbo.usp_checkMerchantCredentials",
            command =>
            {
                command.Parameters.AddWithValue("@var_MerchantID", merchantCredentials.MerchantId);
                command.Parameters.AddWithValue("@var_MerchantPassword", merchantCredentials.MerchantPassword);
            },
            async reader => await HandleCheckMerchantCredentialsResponse(reader));
    }

    private static async Task<ResponseModel<object>> HandleRegisterCustomerResponse(SqlDataReader reader)
    {
        if (!await reader.ReadAsync().ConfigureAwait(false))
            return new ResponseModel<object>(500, "No data returned or failed to process request.");

        return reader["ReturnValue"] is int returnValue
            ? returnValue switch
            {
                200 => new ResponseModel<object>(200, "Customer created successfully!"),
                4001 => new ResponseModel<object>(400, "MSISDN already exists."),
                4002 => new ResponseModel<object>(400, "Email already exists."),
                _ => new ResponseModel<object>(500, "Internal error occurred.")
            }
            : new ResponseModel<object>(500, "Failed to create customer.");
    }

    private static async Task<ResponseModel<object>> HandleGetCustomerResponse(SqlDataReader reader)
    {
        if (!await reader.ReadAsync().ConfigureAwait(false))
            return new ResponseModel<object>(404, "Customer not found");

        return new ResponseModel<object>(200, "Customer found.", DbHelper.MapCustomerFromReader(reader));
    }

    private static async Task<ResponseModel<object>> HandleGetCustomersResponse(SqlDataReader reader)
    {
        var customers = new List<CustomerModel>();

        while (await reader.ReadAsync().ConfigureAwait(false))
            customers.Add(DbHelper.MapCustomerFromReader(reader));

        return new ResponseModel<object>(200, $"{customers.Count} customers found.",
            new CustomerListModel { CustomerList = customers });
    }

    private static async Task<ResponseModel<object>> HandleEditCustomerResponse(SqlDataReader reader)
    {
        if (!await reader.ReadAsync().ConfigureAwait(false))
            return new ResponseModel<object>(500, "No data returned or customer update failed.");

        var message = reader["message"] as string;

        return reader["result"] is 1
            ? new ResponseModel<object>(200, message ?? "Customer details updated successfully!")
            : new ResponseModel<object>(500, message ?? "Failed to update customer.");
    }

    private static async Task<ResponseModel<object>> HandleDeactivateOrDeleteCustomerResponse(SqlDataReader reader)
    {
        if (!await reader.ReadAsync().ConfigureAwait(false))
            return new ResponseModel<object>(500, "Customer not found or no data returned from the procedure.");

        var message = reader["message"] as string;

        return reader["result"] is 1
            ? new ResponseModel<object>(200, message ?? "Operation successful!")
            : new ResponseModel<object>(500, message ?? "Operation failed.");
    }

    private static async Task<ResponseModel<object>> HandleCheckMerchantCredentialsResponse(SqlDataReader reader)
    {
        var result = new ResultValidityCheck { IsValid = false };

        if (!await reader.ReadAsync().ConfigureAwait(false))
            return new ResponseModel<object>(404, "Invalid merchant credentials or no matching merchant found!",
                result);

        if (reader["merchant_role"] is int role)
        {
            result.IsValid = role == 1801;
            result.ErrorMessage = result.IsValid ? null : "The provided merchant credentials have invalid roles!";
        }
        else
        {
            result.ErrorMessage = "Invalid merchant credentials or no matching merchant found!";
        }

        return result.IsValid
            ? new ResponseModel<object>(200, "Valid merchant credentials.", result)
            : new ResponseModel<object>(400, result.ErrorMessage, result);
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
        try
        {
            CheckConnectionString();

            await using var connection = new SqlConnection(CurrentConnectionString);
            await connection.OpenAsync().ConfigureAwait(false);

            await using var command = new SqlCommand(storedProcedure, connection);
            command.CommandType = CommandType.StoredProcedure;

            configureCommand?.Invoke(command);

            await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
            return await handleReader(reader);
        }
        catch (SqlException sqlEx)
        {
            throw new InvalidOperationException(
                $"Error executing stored procedure '{storedProcedure}': {sqlEx.Message}", sqlEx);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Unexpected error during stored procedure execution: {ex.Message}",
                ex);
        }
    }
}