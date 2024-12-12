using Microsoft.Extensions.Configuration;

namespace CustomerManagementSystem.BusinessLogic.Configuration;

public class BllConfig(IConfiguration configuration) : IBllConfig
{
    public string SecureJwtKey => configuration["Auth:SecureJWTKey"] ??
                                  throw new ArgumentNullException(nameof(SecureJwtKey),
                                      "The config value SecureJWTKey cannot be null.");

    public string JwtIssuer => configuration["Auth:JWTIssuer"] ??
                               throw new ArgumentNullException(nameof(JwtIssuer),
                                   "The config value JWTIssuer cannot be null.");

    public string JwtAudience => configuration["Auth:JWTAudience"] ??
                                 throw new ArgumentNullException(nameof(JwtAudience),
                                     "The config value JWTAudience cannot be null.");

    public string AccessTokenTimeout => configuration["Auth:AccessTokenTimeout"] ??
                                        throw new ArgumentNullException(nameof(AccessTokenTimeout),
                                            "The config value AccessTokenTimeout cannot be null.");
}