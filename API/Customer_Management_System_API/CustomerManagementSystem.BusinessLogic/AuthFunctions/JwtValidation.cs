using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CustomerManagementSystem.Domain.Configuration;
using CustomerManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace CustomerManagementSystem.BusinessLogic.AuthFunctions;

public class JwtValidation
{
    private readonly string _jwtAudience;
    private readonly string _jwtIssuer;
    private readonly string _jwtKey;

    public JwtValidation(IAppSettingsConfig appSettingsConfig)
    {
        _jwtKey = appSettingsConfig.SecureJwtKey;
        _jwtIssuer = appSettingsConfig.JwtIssuer;
        _jwtAudience = appSettingsConfig.JwtAudience;
    }

    public ResponseModel<object> Authorize(HttpContext httpContext, string? bearerToken)
    {
        string authHeader;
        if (string.IsNullOrEmpty(bearerToken))
            authHeader = httpContext.Request.Headers["Authorization"].ToString();
        else
            authHeader = "Bearer " + bearerToken;

        if (string.IsNullOrEmpty(authHeader)) return new ResponseModel<object>(500, "Unauthorized: Empty auth header.");

        if (!authHeader.StartsWith("Bearer "))
            return new ResponseModel<object>(500, "Unauthorized: No Bearer header identified.");

        var jwt = authHeader.Split(' ')[1];
        return string.IsNullOrEmpty(jwt)
            ? new ResponseModel<object>(500, "Unauthorized: Empty JWT.")
            : ValidateToken(jwt);
    }

    private ResponseModel<object> ValidateToken(string? token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtKey);
        try
        {
            if (!tokenHandler.CanReadToken(token))
                return new ResponseModel<object>(500, "Unauthorized: No JWT of a valid format was provided.");

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtIssuer,
                ValidateAudience = true,
                ValidAudience = _jwtAudience,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            return !VerifyClaims(jwtToken)
                ? new ResponseModel<object>(500, "Unauthorized: Invalid claims.")
                : new ResponseModel<object>(200, "Authorized: Valid claims.");
        }
        catch (Exception ex)
        {
            return new ResponseModel<object>(StatusCodes.Status500InternalServerError,
                $"Unauthorized: An error occurred while validating the access token: {ex.Message}");
        }
    }

    private static bool VerifyClaims(JwtSecurityToken jwtToken)
    {
        return jwtToken.Claims.Any() &&
               jwtToken.Claims.Any(x => x.Type == ClaimTypes.Sid && !string.IsNullOrEmpty(x.Value));
    }
}