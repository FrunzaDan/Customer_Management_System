using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Configuration;
using CustomerManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace CustomerManagementSystem.BusinessLogic.AuthFunctions;

public class JwtCreation
{
    private readonly IAppSettingsConfig _configuration;
    private readonly IDbUtils _dbUtils;
    private readonly byte[] _jwtKey;

    public JwtCreation(IAppSettingsConfig appSettingsConfig, IDbUtils dbUtils)
    {
        _dbUtils = dbUtils;
        _configuration = appSettingsConfig;
        _jwtKey = Encoding.ASCII.GetBytes(_configuration.SecureJwtKey);
    }

    public async Task<ResponseModel<object>> GenerateBearerJwt(MerchantCredentials merchantCredentials)
    {
        if (string.IsNullOrWhiteSpace(merchantCredentials.MerchantId))
            return new ResponseModel<object>(403, "Invalid or empty merchant ID.");

        try
        {
            // Validate merchant credentials
            var credentialsCheck = await _dbUtils.CheckMerchantCredentialsFromDb(merchantCredentials);

            if (credentialsCheck.Status != 200)
                return new ResponseModel<object>(403,
                    credentialsCheck.ResponseMessage);

            // Generate token
            var token = GenerateJwtToken(merchantCredentials.MerchantId);

            if (!double.TryParse(_configuration.AccessTokenTimeout, out var timeoutMinutes))
                return new ResponseModel<object>(500, "Invalid AccessTokenTimeout configuration.");

            return new ResponseModel<object>
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
            return new ResponseModel<object>(500, $"An error occurred: {ex.Message}");
        }
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
}