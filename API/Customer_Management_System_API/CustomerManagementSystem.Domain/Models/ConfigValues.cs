namespace CustomerManagementSystem.Domain.Models
{
    public sealed class ConfigValues
    {
        public string CustomerManagementSystemDB_Windows { get; }
        public string CustomerManagementSystemDB_Docker { get; }
        public string SecureJWTKey { get; }
        public string JWTIssuer { get; }
        public string JWTAudience { get; }
        public string AccessTokenTimeout { get; }

        public ConfigValues(
            string customerManagementSystemDB_Windows,
            string customerManagementSystemDB_Docker,
            string secureJWTKey,
            string jwtIssuer,
            string jwtAudience,
            string accessTokenTimeout)
        {
            CustomerManagementSystemDB_Windows = customerManagementSystemDB_Windows;
            CustomerManagementSystemDB_Docker = customerManagementSystemDB_Docker;
            SecureJWTKey = secureJWTKey;
            JWTIssuer = jwtIssuer;
            JWTAudience = jwtAudience;
            AccessTokenTimeout = accessTokenTimeout;
        }
    }
}