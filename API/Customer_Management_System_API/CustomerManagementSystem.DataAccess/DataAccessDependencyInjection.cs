using CustomerManagementSystem.DataAccess.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerManagementSystem.DataAccess;

public static class DataAccessDependencyInjection
{
    public static void AddDataAccess(this IServiceCollection services)
    {
        services.AddSingleton<IDalConfig, DalConfig>();
    }
}