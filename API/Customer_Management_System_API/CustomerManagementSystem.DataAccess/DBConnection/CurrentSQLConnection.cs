using CustomerManagementSystem.DataAccess.Configuration;
using Microsoft.Data.SqlClient;

namespace CustomerManagementSystem.DataAccess.DBConnection;

public class CurrentSqlConnection
{
    private readonly IDalConfig _configuration;

    public CurrentSqlConnection(IDalConfig configuration)
    {
        _configuration = configuration;
    }

    public string GetCorrectSqlConnectionString()
    {
        string connectionString;

        if (CheckSqlConnection(_configuration.CustomerManagementSystemDbDocker))
        {
            connectionString = _configuration.CustomerManagementSystemDbDocker;
        }
        else if (CheckSqlConnection(_configuration.CustomerManagementSystemDbWindows))
        {
            connectionString = _configuration.CustomerManagementSystemDbWindows;
        }
        else
        {
            throw new InvalidOperationException("No valid SQL connection could be established.");
        }

        return connectionString;
    }

    private static bool CheckSqlConnection(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return false;
        }

        try
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                return true;
            }
        }
        catch (SqlException)
        {
            return false;
        }
    }
}