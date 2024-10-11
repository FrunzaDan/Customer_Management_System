using Microsoft.Extensions.Configuration;

namespace CustomerManagementSystem.DataAccess.Configuration
{
    public class DALConfig : IDALConfig
    {
        private readonly IConfiguration _configuration;

        public DALConfig(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CustomerManagementSystemDB_Windows => _configuration["ConnectionStrings:CustomerManagementSystemDB_Windows"] ?? throw new ArgumentNullException(nameof(CustomerManagementSystemDB_Windows), "The config value CustomerManagementSystemDB_Windows cannot be null.");
        public string CustomerManagementSystemDB_Docker => _configuration["ConnectionStrings:CustomerManagementSystemDB_Docker"] ?? throw new ArgumentNullException(nameof(CustomerManagementSystemDB_Docker), "The config value CustomerManagementSystemDB_Docker cannot be null.");
    }
}