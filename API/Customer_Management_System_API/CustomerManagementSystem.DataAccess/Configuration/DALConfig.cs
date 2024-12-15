using Microsoft.Extensions.Configuration;

namespace CustomerManagementSystem.DataAccess.Configuration;

public class DalConfig : IDalConfig
{
    private readonly IConfiguration _configuration;

    public DalConfig(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string? CustomerManagementSystemDbWindows =>
        _configuration["ConnectionStrings:CustomerManagementSystemDB_Windows"] ?? throw new ArgumentNullException(
            nameof(CustomerManagementSystemDbWindows),
            "The config value CustomerManagementSystemDB_Windows cannot be null.");

    public string? CustomerManagementSystemDbDocker =>
        _configuration["ConnectionStrings:CustomerManagementSystemDB_Docker"] ?? throw new ArgumentNullException(
            nameof(CustomerManagementSystemDbDocker),
            "The config value CustomerManagementSystemDB_Docker cannot be null.");
}