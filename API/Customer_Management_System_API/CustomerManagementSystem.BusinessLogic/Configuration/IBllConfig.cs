namespace CustomerManagementSystem.BusinessLogic.Configuration;

public interface IBllConfig
{
    string SecureJwtKey { get; }
    string JwtIssuer { get; }
    string JwtAudience { get; }
    string AccessTokenTimeout { get; }
}