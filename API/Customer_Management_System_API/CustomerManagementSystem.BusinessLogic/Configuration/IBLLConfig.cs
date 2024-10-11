namespace CustomerManagementSystem.BusinessLogic.Configuration
{
    public interface IBLLConfig
    {
        string CustomerManagementSystemDB_Windows { get; }
        string CustomerManagementSystemDB_Docker { get; }
        string SecureJWTKey { get; }
        string JWTIssuer { get; }
        string JWTAudience { get; }
        string AccessTokenTimeout { get; }
    }
}