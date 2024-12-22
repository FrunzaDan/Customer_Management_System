using CustomerManagementSystem.BusinessLogic.Configuration;
using CustomerManagementSystem.BusinessLogic.Services;
using CustomerManagementSystem.BusinessLogic.Services.Implementation;
using CustomerManagementSystem.DataAccess.DBConnection;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerManagementSystem.BusinessLogic;

public static class BusinessLogicDependencyInjection
{
    public static void AddBusinessLogic(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddSingleton<IDbUtils, DbUtils>();
        services.AddSingleton<IBllConfig, BllConfig>();
    }
}