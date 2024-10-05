using CustomerManagementSystem.BusinessLogic.Services;
using CustomerManagementSystem.BusinessLogic.Services.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerManagementSystem.BusinessLogic;

public static class BusinessLogicDependencyInjection
{
    public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
    {
        services.AddServices();

        return services;
    }

    private static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
    }
}