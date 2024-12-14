using CustomerManagementSystem.Domain.Models;
using Microsoft.Data.SqlClient;

namespace CustomerManagementSystem.DataAccess.DBConnection;

public class DbHelper
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
}