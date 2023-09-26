using System;
namespace Customer_Management_System_Library.Configuration
{
    public interface ICMSConfig
    {
        string CustomerManagementSystemDB { get; }

        string SecureJWTKey { get; }

        string AccessTokenTimeout { get; }
    }
}

