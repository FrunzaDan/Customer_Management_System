using System.Data;
using CustomerManagementSystem.DataAccess.Configuration;
using CustomerManagementSystem.Domain.Models;
using Microsoft.Data.SqlClient;

namespace CustomerManagementSystem.DataAccess.DBConnection;

public class DbUtils : IDbUtils
{
    private readonly IDalConfig _configuration;

    public DbUtils(IDalConfig configuration)
    {
        _configuration = configuration;
    }

    private string? CurrentConnectionString { get; set; }

    public async Task<ResponseModel> RegisterCustomer(CustomerModel customer)
    {
        const string storedProcedure = "dbo.usp_createCustomer";
        return await ExecuteStoredProcedureAsync<ResponseModel>(storedProcedure,
            command => { DbHelper.AddCustomerParameters(command, customer); },
            async reader => await HandleResponseAsync(reader, "Success", "Conflict occurred.", "Internal Error"));
    }

    public async Task<CustomerModel> GetCustomer(GetCustomerRequest request)
    {
        const string storedProcedure = "dbo.usp_getCustomer";
        return await ExecuteStoredProcedureAsync<CustomerModel>(storedProcedure, command =>
            {
                command.Parameters.AddWithValue("@var_SearchOption", request.SearchOption);
                command.Parameters.AddWithValue("@var_SearchVariable", request.SearchVariable ?? (object)DBNull.Value);
            },
            async reader =>
            {
                return await reader.ReadAsync().ConfigureAwait(false)
                    ? DbHelper.MapCustomerFromReader(reader)
                    : new CustomerModel { Status = 404, ResponseMessage = "Customer not found" };
            });
    }

    public async Task<CustomerListModel> GetCustomers()
    {
        const string storedProcedure = "dbo.usp_getCustomers";
        return await ExecuteStoredProcedureAsync<CustomerListModel>(storedProcedure, null, async reader =>
        {
            var customers = new List<CustomerModel>();
            while (await reader.ReadAsync().ConfigureAwait(false))
                customers.Add(DbHelper.MapCustomerFromReader(reader));

            return new CustomerListModel
            {
                CustomerList = customers,
                Status = 200,
                ResponseMessage = $"{customers.Count} customers found."
            };
        });
    }

    public async Task<ResponseModel> EditCustomer(CustomerModel customer)
    {
        const string storedProcedure = "dbo.usp_editCustomer";
        return await ExecuteStoredProcedureAsync<ResponseModel>(storedProcedure,
            command => { DbHelper.AddCustomerParameters(command, customer); }, async reader =>
            {
                if (await reader.ReadAsync().ConfigureAwait(false))
                {
                    var customerStatus = reader["customer_Status"].ToString();
                    if (int.TryParse(customerStatus, out var customerStatusInt) &&
                        (customerStatusInt == 1901 || customerStatusInt == 1903))
                        return new ResponseModel
                        {
                            Status = 200,
                            ResponseMessage = "Customer edited successfully!"
                        };

                    return new ResponseModel
                    {
                        Status = 500,
                        ResponseMessage = "Could not read customer status code!"
                    };
                }

                return new ResponseModel
                {
                    Status = 500,
                    ResponseMessage = "Couldn't read ResponseCode"
                };
            });
    }

    public async Task<ResponseModel> DeactivateCustomer(string customerGuid)
    {
        const string storedProcedure = "dbo.usp_deactivateCustomer";
        return await ExecuteStoredProcedureAsync<ResponseModel>(storedProcedure,
            command => { command.Parameters.AddWithValue("@var_Guid", customerGuid); }, async reader =>
            {
                if (await reader.ReadAsync().ConfigureAwait(false))
                    return await ParseStatus(reader, 1903, "Customer deactivated successfully!");

                return new ResponseModel
                {
                    Status = 500,
                    ResponseMessage = "Couldn't read ResponseCode"
                };
            });
    }

    public async Task<ResponseModel> DeleteCustomer(string customerGuid)
    {
        const string storedProcedure = "dbo.usp_deleteCustomer";
        return await ExecuteStoredProcedureAsync<ResponseModel>(storedProcedure,
            command => { command.Parameters.AddWithValue("@var_Guid", customerGuid); }, async reader =>
            {
                if (await reader.ReadAsync().ConfigureAwait(false))
                    return await ParseStatus(reader, 1903, "Customer deleted successfully!");

                return new ResponseModel
                {
                    Status = 500,
                    ResponseMessage = "Couldn't read ResponseCode"
                };
            });
    }

    public async Task<ResultValidityCheck> CheckMerchantCredentialsFromDb(MerchantCredentials merchantCredentials)
    {
        const string storedProcedure = "dbo.usp_checkMerchantCredentials";
        return await ExecuteStoredProcedureAsync<ResultValidityCheck>(storedProcedure, command =>
        {
            command.Parameters.AddWithValue("@var_MerchantID", merchantCredentials.MerchantId);
            command.Parameters.AddWithValue("@var_MerchantPassword", merchantCredentials.MerchantPassword);
        }, async reader =>
        {
            var result = new ResultValidityCheck { IsValid = false };
            if (await reader.ReadAsync().ConfigureAwait(false))
            {
                result.IsValid = int.TryParse(reader["merchant_role"].ToString(), out var role) && role == 1801;
                if (!result.IsValid) result.ErrorMessage = "The provided merchant credentials had invalid roles!";
            }
            else
            {
                result.ErrorMessage = "No matching merchant credentials found!";
            }

            return result;
        });
    }

    private void CheckConnectionString()
    {
        if (!string.IsNullOrEmpty(CurrentConnectionString)) return;
        var currentSqlConnection = new CurrentSqlConnection(_configuration);
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

        configureCommand?.Invoke(command);

        await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
        return await handleReader(reader);
    }

    private async Task<ResponseModel> HandleResponseAsync(SqlDataReader reader, string successMessage,
        string conflictMessage, string errorMessage)
    {
        if (await reader.ReadAsync().ConfigureAwait(false))
            return new ResponseModel
            {
                Status = reader.GetInt32("ReturnValue") == 200 ? 200 : 409,
                ResponseMessage = reader.GetInt32("ReturnValue") == 200 ? successMessage : conflictMessage
            };

        return new ResponseModel
        {
            Status = 500,
            ResponseMessage = errorMessage
        };
    }

    private async Task<ResponseModel> ParseStatus(SqlDataReader reader, int expectedStatus, string successMessage)
    {
        if (await reader.ReadAsync().ConfigureAwait(false))
        {
            if (int.TryParse(reader["customer_Status"].ToString(), out var customerStatusInt) &&
                customerStatusInt == expectedStatus)
                return new ResponseModel
                {
                    Status = 200,
                    ResponseMessage = successMessage
                };

            return new ResponseModel
            {
                Status = 500,
                ResponseMessage = "Could not read customer status code!"
            };
        }

        return new ResponseModel
        {
            Status = 500,
            ResponseMessage = "Couldn't read ResponseCode"
        };
    }
}