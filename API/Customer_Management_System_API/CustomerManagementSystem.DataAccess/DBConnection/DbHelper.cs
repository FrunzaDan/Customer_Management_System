using CustomerManagementSystem.Domain.Models;
using Microsoft.Data.SqlClient;

namespace CustomerManagementSystem.DataAccess.DBConnection;

public static class DbHelper
{
    public static CustomerModel MapCustomerFromReader(SqlDataReader reader)
    {
        var customer = new CustomerModel
        {
            Guid = reader["PK_customer_guid"]?.ToString(),
            FirstName = reader["first_name"]?.ToString(),
            LastName = reader["last_name"]?.ToString(),
            Email = reader["email"]?.ToString(),
            Msisdn = reader["msisdn"]?.ToString(),
            Birthdate = reader["birthDate"]?.ToString(),
            Address = new AddressModel
            {
                Country = reader["country"]?.ToString(),
                County = reader["county"]?.ToString(),
                Town = reader["town"]?.ToString(),
                Zip = reader["zip_code"]?.ToString(),
                Street = reader["street"]?.ToString(),
                Number = reader["number"]?.ToString()
            },
            Gender = int.TryParse(reader["gender"]?.ToString(), out var gender) ? gender : null,
            CustomerStatus = int.TryParse(reader["customer_Status"]?.ToString(), out var status) ? status : null
        };

        return customer;
    }

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