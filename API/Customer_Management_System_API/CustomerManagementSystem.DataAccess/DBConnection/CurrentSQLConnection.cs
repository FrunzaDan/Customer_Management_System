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

    public SqlConnection CreateCurrentSqlConnection()
    {
        var sqlConnection = new SqlConnection();
        sqlConnection.ConnectionString = _configuration.CustomerManagementSystemDbDocker;
        if (CheckSqlConnection(sqlConnection) == false)
            sqlConnection.ConnectionString = _configuration.CustomerManagementSystemDbWindows;
        return sqlConnection;
    }

    private bool CheckSqlConnection(SqlConnection sqlConnection)
    {
        var isConnected = false;
        try
        {
            sqlConnection.Open();
            sqlConnection.Close();
            isConnected = true;
            return isConnected;
        }
        catch (SqlException)
        {
            return isConnected;
        }
    }
}