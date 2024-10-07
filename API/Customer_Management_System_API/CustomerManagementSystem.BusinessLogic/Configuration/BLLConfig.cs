using Microsoft.Extensions.Configuration;

namespace CustomerManagementSystem.BusinessLogic.Configuration
{
    public class BLLConfig : IBLLConfig
    {
        private readonly IConfiguration _configuration;

        public BLLConfig(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string SecureJWTKey => _configuration["Auth:SecureJWTKey"] ?? throw new ArgumentNullException(nameof(SecureJWTKey), "The config value SecureJWTKey cannot be null.");
        public string JWTIssuer => _configuration["Auth:JWTIssuer"] ?? throw new ArgumentNullException(nameof(JWTIssuer), "The config value JWTIssuer cannot be null.");
        public string JWTAudience => _configuration["Auth:JWTAudience"] ?? throw new ArgumentNullException(nameof(JWTAudience), "The config value JWTAudience cannot be null.");
        public string AccessTokenTimeout => _configuration["Auth:AccessTokenTimeout"] ?? throw new ArgumentNullException(nameof(AccessTokenTimeout), "The config value AccessTokenTimeout cannot be null.");
    }
}