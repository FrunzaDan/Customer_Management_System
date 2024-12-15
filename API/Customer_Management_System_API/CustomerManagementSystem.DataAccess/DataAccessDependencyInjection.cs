using CustomerManagementSystem.DataAccess.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerManagementSystem.DataAccess;

public static class DataAccessDependencyInjection
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services)
    {
        services.AddSingleton<IDalConfig, DalConfig>();
        return services;
    }
}