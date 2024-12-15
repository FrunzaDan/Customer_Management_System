using CustomerManagementSystem.DataAccess.Configuration;
using Microsoft.Data.SqlClient;

namespace CustomerManagementSystem.DataAccess.DBConnection;

public class CurrentSqlConnection
{
    private readonly IDalConfig _configuration;

    public CurrentSqlConnection(IDalConfig configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public string? GetCorrectSqlConnectionString()
    {
        return GetValidConnectionString(
            _configuration.CustomerManagementSystemDbDocker,
            _configuration.CustomerManagementSystemDbWindows);
    }

    private static string? GetValidConnectionString(params string?[] connectionStrings)
    {
        foreach (var connectionString in connectionStrings)
            if (IsConnectionValid(connectionString))
                return connectionString;

        return null;
    }

    private static bool IsConnectionValid(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            return false;

        try
        {
            using var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            return sqlConnection.State == System.Data.ConnectionState.Open;
        }
        catch (SqlException)
        {
            return false;
        }
        catch
        {
            return false;
        }
    }

}