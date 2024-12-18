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
        command =>
        {
            DbHelper.AddCustomerParameters(command, customer);  // Assuming this method adds all necessary parameters
        },
        async reader =>
        {
            if (await reader.ReadAsync().ConfigureAwait(false))
            {
                var returnValue = reader["ReturnValue"] as int?;

                // Check return value and handle accordingly
                if (returnValue.HasValue)
                {
                    switch (returnValue.Value)
                    {
                        case 200:
                            return new ResponseModel
                            {
                                Status = 200,
                                ResponseMessage = "Customer created successfully!"
                            };
                        case 4001:
                            return new ResponseModel
                            {
                                Status = 400,
                                ResponseMessage = "MSISDN already exists."
                            };
                        case 4002:
                            return new ResponseModel
                            {
                                Status = 400,
                                ResponseMessage = "Email already exists."
                            };
                        default:
                            return new ResponseModel
                            {
                                Status = 500,
                                ResponseMessage = "Internal error occurred."
                            };
                    }
                }

                return new ResponseModel
                {
                    Status = 500,
                    ResponseMessage = "Failed to create customer."
                };
            }

            return new ResponseModel
            {
                Status = 500,
                ResponseMessage = "No data returned or failed to process request."
            };
        });
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
            command =>
            {
                DbHelper.AddCustomerParameters(command, customer); 
            },
            async reader =>
            {
                if (await reader.ReadAsync().ConfigureAwait(false))
                {
                    var result = reader["result"] as int?;
                    var message = reader["message"] as string;

                    if (result.HasValue && result.Value == 1)
                    {
                        return new ResponseModel
                        {
                            Status = 200,
                            ResponseMessage = message ?? "Customer details updated successfully!"
                        };
                    }

                    return new ResponseModel
                    {
                        Status = 500,
                        ResponseMessage = message ?? "Failed to update customer."
                    };
                }

                return new ResponseModel
                {
                    Status = 500,
                    ResponseMessage = "No data returned or customer update failed."
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
                {
                    var result = reader["result"] as int?;
                    var message = reader["message"] as string;

                    if (result.HasValue && result.Value == 1)
                    {
                        return new ResponseModel
                        {
                            Status = 200,
                            ResponseMessage = message ?? "Failed to deactivate customer!"
                        };
                    }

                    return new ResponseModel
                    {
                        Status = 500,
                        ResponseMessage = "Failed to deactivate customer!"
                    };
                }

                return new ResponseModel
                {
                    Status = 500,
                    ResponseMessage = "Customer not found or no data returned from the procedure."
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
                {
                    var result = reader["result"] as int?;
                    var message = reader["message"] as string;

                    if (result.HasValue && result.Value == 1)
                    {
                        return new ResponseModel
                        {
                            Status = 200,
                            ResponseMessage = message ?? "Customer deleted successfully!"
                        };
                    }

                    return new ResponseModel
                    {
                        Status = 500,
                        ResponseMessage = message ?? "Failed to delete customer."
                    };
                }

                return new ResponseModel
                {
                    Status = 500,
                    ResponseMessage = "Customer not found or no data returned from the procedure."
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
                var role = reader["merchant_role"] as int?;

                if (role.HasValue)
                {
                    result.IsValid = role == 1801;
                    if (!result.IsValid)
                    {
                        result.ErrorMessage = "The provided merchant credentials have invalid roles!";
                    }
                }
                else
                {
                    result.ErrorMessage = "Invalid merchant credentials or no matching merchant found!";
                }
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

        // If the configureCommand delegate is provided, invoke it to allow for custom configuration of the SqlCommand
        configureCommand?.Invoke(command);

        await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
        
        // Pass the SqlDataReader to the handleReader function to process the data and return a result of type T.
        return await handleReader(reader);
    }
}