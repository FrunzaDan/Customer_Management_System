using Microsoft.Extensions.Configuration;

namespace CustomerManagementSystem.DataAccess.Configuration;

public class DalConfig(IConfiguration configuration) : IDalConfig
{
    public string CustomerManagementSystemDbWindows =>
        configuration["ConnectionStrings:CustomerManagementSystemDB_Windows"] ?? throw new ArgumentNullException(
            nameof(CustomerManagementSystemDbWindows),
            "The config value CustomerManagementSystemDB_Windows cannot be null.");

    public string CustomerManagementSystemDbDocker =>
        configuration["ConnectionStrings:CustomerManagementSystemDB_Docker"] ?? throw new ArgumentNullException(
            nameof(CustomerManagementSystemDbDocker),
            "The config value CustomerManagementSystemDB_Docker cannot be null.");
}