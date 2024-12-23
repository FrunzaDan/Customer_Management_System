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
    private readonly byte[] _jwtKey;

    public JwtCreation()
    {
        _configuration = ServiceLocator.GetServiceFromServiceProvider<IBllConfig>();
        _dbUtils = ServiceLocator.GetServiceFromServiceProvider<IDbUtils>();
        _jwtKey = Encoding.ASCII.GetBytes(_configuration.SecureJwtKey);
    }

    public async Task<ResponseModel<AccessTokenResponse>> GenerateBearerJwt(string merchantId, string merchantPassword)
    {
        if (string.IsNullOrWhiteSpace(merchantId) || string.IsNullOrWhiteSpace(merchantPassword))
            return CreateErrorResponse(StatusCodes.Status400BadRequest, "Merchant ID and Password cannot be empty.");

        try
        {
            // Validate merchant credentials
            var credentialsCheck = await ValidateMerchantCredentials(merchantId, merchantPassword);
            if (credentialsCheck is { Data.IsValid: false, Data.ErrorMessage: not null })
                return CreateErrorResponse(StatusCodes.Status403Forbidden, credentialsCheck.Data.ErrorMessage);

            // Generate token
            var token = GenerateJwtToken(merchantId);
            if (string.IsNullOrEmpty(token))
            {
                return new ResponseModel<AccessTokenResponse>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    ResponseMessage = "Failed to generate JWT token."
                };
            }
            
            if (string.IsNullOrEmpty(_configuration.AccessTokenTimeout))
            {
                return new ResponseModel<AccessTokenResponse>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    ResponseMessage = "Configuration error: AccessTokenTimeout is missing."
                };
            }
            
            if (!double.TryParse(_configuration.AccessTokenTimeout, out var timeoutMinutes))
            {
                return new ResponseModel<AccessTokenResponse>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    ResponseMessage = "Invalid AccessTokenTimeout configuration."
                };
            }

            return new ResponseModel<AccessTokenResponse>
            {
                Status = StatusCodes.Status200OK,
                ResponseMessage = "Success!",
                Data = new AccessTokenResponse
                {
                    AccessToken = token,
                    ValidUntil = DateTime.UtcNow.AddMinutes(timeoutMinutes).ToString("o")
                }
            };
        }
        catch (Exception ex)
        {
            return CreateErrorResponse(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
        }
    }

    private async Task<ResponseModel<ResultValidityCheck>> ValidateMerchantCredentials(string merchantId, string merchantPassword)
    {
        var merchantCredentials = new MerchantCredentials
        {
            MerchantId = merchantId,
            MerchantPassword = merchantPassword
        };

        return await _dbUtils.CheckMerchantCredentialsFromDb(merchantCredentials);
    }

    private string GenerateJwtToken(string merchantId)
    {
        var tokenDescriptor = BuildTokenDescriptor(merchantId);
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private SecurityTokenDescriptor BuildTokenDescriptor(string merchantId)
    {
        return new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.Sid, merchantId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString("o"))
            ]),
            Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration.AccessTokenTimeout)),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(_jwtKey), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration.JwtIssuer,
            Audience = _configuration.JwtAudience
        };
    }

    private static ResponseModel<AccessTokenResponse> CreateErrorResponse(int statusCode, string message)
    {
        return new ResponseModel<AccessTokenResponse>
        {
            Status = statusCode,
            ResponseMessage = message,
            Data = null
        };
    }
}