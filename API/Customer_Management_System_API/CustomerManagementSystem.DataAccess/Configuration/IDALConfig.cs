namespace CustomerManagementSystem.DataAccess.Configuration;

public interface IDalConfig
{
    string CustomerManagementSystemDbWindows { get; }
    string CustomerManagementSystemDbDocker { get; }
}