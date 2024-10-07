namespace CustomerManagementSystem.BusinessLogic.Configuration
{
    public interface IBLLConfig
    {
        string SecureJWTKey { get; }
        string JWTIssuer { get; }
        string JWTAudience { get; }
        string AccessTokenTimeout { get; }
    }
}