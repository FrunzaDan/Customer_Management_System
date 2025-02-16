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
            DbHelper.HandleResponseWithMessage
        );
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
            reader => DbHelper.HandleResponseWithCustomerMapping(reader, "Customer found.",
                "Customer not found")
        );
    }

    public async Task<ResponseModel<object>> GetCustomers()
    {
        return await ExecuteStoredProcedureAsync(
            "dbo.usp_getCustomers",
            null,
            reader => DbHelper.HandleResponseWithList(reader, "customers")
        );
    }

    public async Task<ResponseModel<object>> EditCustomer(CustomerModel customer)
    {
        return await ExecuteStoredProcedureAsync(
            "dbo.usp_editCustomer",
            command => DbHelper.AddCustomerParameters(command, customer),
            DbHelper.HandleResponseWithMessage
        );
    }

    public async Task<ResponseModel<object>> DeactivateCustomer(string customerGuid)
    {
        return await ExecuteStoredProcedureAsync(
            "dbo.usp_deactivateCustomer",
            command => command.Parameters.AddWithValue("@var_Guid", customerGuid),
            DbHelper.HandleResponseWithMessage
        );
    }
    
    public async Task<ResponseModel<object>> ReactivateCustomer(string customerGuid)
    {
        return await ExecuteStoredProcedureAsync(
            "dbo.usp_reactivateCustomer",
            command => command.Parameters.AddWithValue("@var_Guid", customerGuid),
            DbHelper.HandleResponseWithMessage
        );
    }

    public async Task<ResponseModel<object>> DeleteCustomer(string customerGuid)
    {
        return await ExecuteStoredProcedureAsync(
            "dbo.usp_deleteCustomer",
            command => command.Parameters.AddWithValue("@var_Guid", customerGuid),
            DbHelper.HandleResponseWithMessage
        );
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
            DbHelper.HandleMerchantCredentialsResponse
        );
    }

    private void CheckConnectionString()
    {
        if (!string.IsNullOrEmpty(CurrentConnectionString)) return;
        CurrentConnectionString = new CurrentSqlConnection(configuration).GetCorrectSqlConnectionString();
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