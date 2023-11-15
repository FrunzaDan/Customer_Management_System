using System;
using Microsoft.Extensions.Configuration;

namespace Customer_Management_System_Library.Configuration
{
    public class CMSConfig : ICMSConfig
    {
        private readonly IConfiguration _configuration;

        public CMSConfig(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CustomerManagementSystemDB => _configuration["ConnectionStrings:CustomerManagementSystemDB"] ?? "Error";

        public string SecureJWTKey => _configuration["Auth:SecureJWTKey"] ?? "Error";

        public string JWTIssuer => _configuration["Auth:JWTIssuer"] ?? "Error";
        public string JWTAudience => _configuration["Auth:JWTAudience"] ?? "Error";

        public string AccessTokenTimeout => _configuration["Auth:AccessTokenTimeout"] ?? "Error";

    }
}

