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

    public async Task<AccessTokenResponse> GenerateBearerJwt(string merchantId, string merchantPassword)
    {
        if (string.IsNullOrWhiteSpace(merchantId) || string.IsNullOrWhiteSpace(merchantPassword))
            return new AccessTokenResponse
            {
                AccessToken = null,
                ValidUntil = null,
                Status = StatusCodes.Status400BadRequest,
                ResponseMessage = "Merchant ID and Password cannot be empty."
            };

        try
        {
            // Validate merchant credentials
            var merchantCredentials = new MerchantCredentials
            {
                MerchantId = merchantId,
                MerchantPassword = merchantPassword
            };

            var credentialsAreValid = await _dbUtils.CheckMerchantCredentialsFromDb(merchantCredentials);
            if (!credentialsAreValid.IsValid)
                return new AccessTokenResponse
                {
                    AccessToken = null,
                    ValidUntil = null,
                    Status = StatusCodes.Status403Forbidden,
                    ResponseMessage = credentialsAreValid.ErrorMessage
                };

            // Generate token
            var tokenDescriptor = BuildTokenDescriptor(merchantId);
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);

            return new AccessTokenResponse
            {
                AccessToken = accessToken,
                ValidUntil = tokenDescriptor.Expires?.ToString("o"),
                Status = StatusCodes.Status200OK,
                ResponseMessage = "Success!"
            };
        }
        catch (Exception ex)
        {
            return new AccessTokenResponse
            {
                AccessToken = null,
                ValidUntil = null,
                Status = StatusCodes.Status500InternalServerError,
                ResponseMessage = $"An error occurred: {ex.Message}"
            };
        }
    }

    private SecurityTokenDescriptor BuildTokenDescriptor(string merchantId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Sid, merchantId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString("o"))
        };

        return new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration.AccessTokenTimeout)),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(_jwtKey), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _jwtIssuer,
            Audience = _jwtAudience
        };
    }
}