using System.Data;
using CustomerManagementSystem.Domain.Configuration;
using Microsoft.Data.SqlClient;

namespace CustomerManagementSystem.DataAccess.DBConnection;

public class CurrentSqlConnection(IAppSettingsConfig configuration)
{
    private readonly IAppSettingsConfig _configuration =
        configuration ?? throw new ArgumentNullException(nameof(configuration));

    public string? GetCorrectSqlConnectionString()
    {
        return GetValidConnectionString(
            _configuration.CustomerManagementSystemDbDocker,
            _configuration.CustomerManagementSystemDbWindows);
    }

    private static string? GetValidConnectionString(params string?[] connectionStrings)
    {
        return connectionStrings.FirstOrDefault(IsConnectionValid);
    }

    private static bool IsConnectionValid(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            return false;

        try
        {
            using var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            return sqlConnection.State == ConnectionState.Open;
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