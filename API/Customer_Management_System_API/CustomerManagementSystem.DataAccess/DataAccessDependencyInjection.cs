using CustomerManagementSystem.BusinessLogic.Configuration;
//using CustomerManagementSystem.BusinessLogic.Services;
//using CustomerManagementSystem.BusinessLogic.Services.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerManagementSystem.DataAccess;

public static class DataAccessDependencyInjection
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services)
    {
        services.AddServices();

        return services;
    }

    private static void AddServices(this IServiceCollection services)
    {
        //services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<IDALConfig, DALConfig>();
    }
}