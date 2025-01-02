using CustomerManagementSystem.Domain.Models;
using Microsoft.Data.SqlClient;

namespace CustomerManagementSystem.DataAccess.DBConnection;

public static class DbHelper
{
    public static void AddCustomerParameters(SqlCommand command, CustomerModel customer)
    {
        command.Parameters.AddWithValue("@var_Guid", customer.Guid ?? Guid.NewGuid().ToString());
        command.Parameters.AddWithValue("@var_FirstName", customer.FirstName);
        command.Parameters.AddWithValue("@var_LastName", customer.LastName);
        command.Parameters.AddWithValue("@var_Email", customer.Email);
        command.Parameters.AddWithValue("@var_MSISDN", customer.Msisdn);
        command.Parameters.AddWithValue("@var_Gender", customer.Gender);
        command.Parameters.AddWithValue("@var_Birthdate", customer.Birthdate);
        AddAddressParameters(command, customer.Address);
    }

    public static async Task<ResponseModel<object>> HandleResponseWithCustomerMapping(SqlDataReader reader,
        string successMessage, string failureMessage)
    {
        if (!await reader.ReadAsync().ConfigureAwait(false))
            return new ResponseModel<object>(404, failureMessage);

        return new ResponseModel<object>(200, successMessage, MapCustomerFromReader(reader));
    }

    public static async Task<ResponseModel<object>> HandleResponseWithList(SqlDataReader reader, string entityName)
    {
        var items = new List<object>();

        while (await reader.ReadAsync().ConfigureAwait(false))
            items.Add(MapCustomerFromReader(reader));

        return new ResponseModel<object>(200, $"{items.Count} {entityName} found.", items);
    }

    public static async Task<ResponseModel<object>> HandleResponseWithMessage(SqlDataReader reader)
    {
        if (!await reader.ReadAsync().ConfigureAwait(false))
            return new ResponseModel<object>(500, "No data returned or operation failed.");

        var message = reader["message"] as string;
        return reader["result"] is 1
            ? new ResponseModel<object>(200, message ?? "Operation successful!")
            : new ResponseModel<object>(500, message ?? "Operation failed.");
    }

    public static async Task<ResponseModel<object>> HandleMerchantCredentialsResponse(SqlDataReader reader)
    {
        if (!await reader.ReadAsync().ConfigureAwait(false))
            return new ResponseModel<object>(404, "Invalid merchant credentials or no matching merchant found!");

        var result = reader["result"] is int resultCode ? resultCode : 500;
        var message = reader["message"] as string ?? "Operation failed.";

        if (result != 0) return new ResponseModel<object>(400, message);

        var role = reader["merchant_role"]?.ToString();
        return role == "1801"
            ? new ResponseModel<object>(200, $"{message} Role: {role}.")
            : new ResponseModel<object>(403, $"The provided merchant role ({role}) is not valid.");
    }


    private static CustomerModel MapCustomerFromReader(SqlDataReader reader)
    {
        var customer = new CustomerModel
        {
            Guid = reader["PK_customer_guid"].ToString(),
            FirstName = reader["first_name"].ToString(),
            LastName = reader["last_name"].ToString(),
            Email = reader["email"].ToString(),
            Msisdn = reader["msisdn"].ToString(),
            CreationDate = reader["creation_Date"].ToString(),
            InteractionDate = reader["interaction_Date"].ToString(),
            Birthdate = reader["birthDate"].ToString(),
            Address = new AddressModel
            {
                Country = reader["country"].ToString(),
                County = reader["county"].ToString(),
                Town = reader["town"].ToString(),
                Zip = reader["zip_code"].ToString(),
                Street = reader["street"].ToString(),
                Number = reader["number"].ToString()
            },
            Gender = int.TryParse(reader["gender"].ToString(), out var gender) ? gender : null,
            CustomerStatus = int.TryParse(reader["customer_Status"].ToString(), out var status) ? status : null
        };

        return customer;
    }

    private static void AddAddressParameters(SqlCommand command, AddressModel? address)
    {
        if (address == null) return;

        command.Parameters.AddWithValue("@var_Country", address.Country ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@var_County", address.County ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@var_Town", address.Town ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@var_ZIP", address.Zip ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@var_Street", address.Street ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@var_Number", address.Number ?? (object)DBNull.Value);
    }
}