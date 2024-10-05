using CustomerManagementSystem.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerManagementSystem.BusinessLogic.Configuration
{
    public static class ConfigProcessing
    {
        public static void ProcessConfigFile(this IServiceCollection services, IConfiguration configuration)
        {
            var configValues = new ConfigValues(
                configuration["ConnectionStrings:CustomerManagementSystemDB_Windows"] ?? throw new ArgumentNullException("CustomerManagementSystemDB_Windows"),
                configuration["ConnectionStrings:CustomerManagementSystemDB_Docker"] ?? throw new ArgumentNullException("CustomerManagementSystemDB_Docker"),
                configuration["Auth:SecureJWTKey"] ?? throw new ArgumentNullException("SecureJWTKey"),
                configuration["Auth:JWTIssuer"] ?? throw new ArgumentNullException("JWTIssuer"),
                configuration["Auth:JWTAudience"] ?? throw new ArgumentNullException("JWTAudience"),
                configuration["Auth:AccessTokenTimeout"] ?? throw new ArgumentNullException("AccessTokenTimeout")
            );

            services.AddSingleton(configValues);
        }
    }
}