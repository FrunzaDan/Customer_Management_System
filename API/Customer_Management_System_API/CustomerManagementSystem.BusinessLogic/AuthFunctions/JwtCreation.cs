using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CustomerManagementSystem.BusinessLogic.Configuration;
using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace CustomerManagementSystem.BusinessLogic.AuthFunctions;

public class JwtCreation
{
    private readonly IBllConfig _configuration;
    private readonly IDbUtils _dbUtils;
    private readonly string _jwtAudience;
    private readonly string _jwtIssuer;
    private readonly byte[] _jwtKey;

    public JwtCreation(IBllConfig configuration, IDbUtils dbUtils)
    {
        _configuration = configuration;
        _jwtKey = Encoding.ASCII.GetBytes(_configuration.SecureJwtKey);
        _jwtIssuer = _configuration.JwtIssuer;
        _jwtAudience = _configuration.JwtAudience;
        _dbUtils = dbUtils;
    }

    public AccessTokenResponse GenerateBearerJwt(string merchantId, string merchantPassword)
    {
        if (string.IsNullOrWhiteSpace(merchantId) || string.IsNullOrWhiteSpace(merchantPassword))
        {
            return new AccessTokenResponse
            {
                AccessToken = null,
                ValidUntil = null,
                ResponseCode = StatusCodes.Status400BadRequest,
                ResponseMessage = "Merchant ID and Password cannot be empty."
            };
        }

        try
        {
            // Validate merchant credentials
            var merchantCredentials = new MerchantCredentials
            {
                MerchantId = merchantId,
                MerchantPassword = merchantPassword
            };

            var credentialsAreValid = _dbUtils.CheckMerchantCredentialsFromDb(merchantCredentials);
            if (!credentialsAreValid)
            {
                return new AccessTokenResponse
                {
                    AccessToken = null,
                    ValidUntil = null,
                    ResponseCode = StatusCodes.Status403Forbidden,
                    ResponseMessage = "Invalid credentials. No Access Rights!"
                };
            }

            // Generate token
            var tokenDescriptor = BuildTokenDescriptor(merchantId);
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);

            return new AccessTokenResponse
            {
                AccessToken = accessToken,
                ValidUntil = tokenDescriptor.Expires?.ToString("o"),
                ResponseCode = StatusCodes.Status200OK,
                ResponseMessage = "Success!"
            };
        }
        catch (Exception ex)
        {
            // Log exception if a logging service is available
            // _logger.LogError(ex, "Error generating JWT");

            return new AccessTokenResponse
            {
                AccessToken = null,
                ValidUntil = null,
                ResponseCode = StatusCodes.Status500InternalServerError,
                ResponseMessage = $"An error occurred: {ex.Message}"
            };
        }
    }

    private SecurityTokenDescriptor BuildTokenDescriptor(string merchantId)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Sid, merchantId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Unique ID for the token
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString("o")) // Issued at
        };

        return new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration.AccessTokenTimeout)),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_jwtKey), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _jwtIssuer,
            Audience = _jwtAudience
        };
    }
}
