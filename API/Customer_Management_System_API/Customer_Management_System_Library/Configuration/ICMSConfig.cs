using System;
namespace Customer_Management_System_Library.Configuration
{
    public interface ICMSConfig
    {
        string CustomerManagementSystemDB { get; }
        string SecureJWTKey { get; }
        string JWTIssuer { get; }
        string JWTAudience { get; }
        string AccessTokenTimeout { get; }
    }
}

