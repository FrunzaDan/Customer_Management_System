using Microsoft.Data.SqlClient;
using CustomerManagementSystem.DataAccess.Configuration;

namespace CustomerManagementSystem.DataAccess.DBConnection
{
    public class CurrentSQLConnection
    {
        public readonly IDALConfig _configuration;

        public CurrentSQLConnection(IDALConfig configuration)
        {
            _configuration = configuration;
        }

        public SqlConnection CreateCurrentSqlConnection()
        {
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = _configuration.CustomerManagementSystemDB_Docker;
            if (CheckSQLConnection(sqlConnection) == false)
            {
                sqlConnection.ConnectionString = _configuration.CustomerManagementSystemDB_Windows;
            }
            return sqlConnection;
        }

        private bool CheckSQLConnection(SqlConnection sqlConnection)
        {
            bool isConnected = false;
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
}