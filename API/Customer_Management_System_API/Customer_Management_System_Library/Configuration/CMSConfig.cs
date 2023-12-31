﻿namespace Customer_Management_System_Library.Configuration
{
    public class CMSConfig : ICMSConfig
    {
        private readonly IConfiguration _configuration;

        public CMSConfig(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CustomerManagementSystemDB_Windows => _configuration["ConnectionStrings:CustomerManagementSystemDB_Windows"] ?? "Error";
        public string CustomerManagementSystemDB_Docker => _configuration["ConnectionStrings:CustomerManagementSystemDB_Docker"] ?? "Error";
        public string SecureJWTKey => _configuration["Auth:SecureJWTKey"] ?? "Error";
        public string JWTIssuer => _configuration["Auth:JWTIssuer"] ?? "Error";
        public string JWTAudience => _configuration["Auth:JWTAudience"] ?? "Error";
        public string AccessTokenTimeout => _configuration["Auth:AccessTokenTimeout"] ?? "Error";
    }
}