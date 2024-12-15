using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CustomerManagementSystem.BusinessLogic.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace CustomerManagementSystem.BusinessLogic.AuthFunctions;

public class JwtValidation
{
    private readonly string _jwtAudience;
    private readonly string _jwtIssuer;
    private readonly string _jwtKey;

    public JwtValidation()
    {
        var configuration = ServiceLocator.GetServiceFromServiceProvider<IBllConfig>();
        _jwtKey = configuration.SecureJwtKey;
        _jwtIssuer = configuration.JwtIssuer;
        _jwtAudience = configuration.JwtAudience;
    }

    public bool Authorize(HttpContext httpContext, string? bearerToken)
    {
        string authHeader;
        if (string.IsNullOrEmpty(bearerToken))
            authHeader = httpContext.Request.Headers["Authorization"].ToString();
        else
            authHeader = "Bearer " + bearerToken;

        if (string.IsNullOrEmpty(authHeader)) return false;

        if (!authHeader.StartsWith("Bearer ")) return false;

        var jwt = authHeader.Split(' ')[1];
        if (string.IsNullOrEmpty(jwt))
            throw new SecurityTokenException("Missing JWT Token in Authorization HTTP Header");

        return ValidateToken(jwt);
    }

    private bool ValidateToken(string? token)
    {
        if (token == null) return false;
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtKey);
        try
        {
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

            var claimsAreValid = VerifyClaims(jwtToken);

            return claimsAreValid;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static bool VerifyClaims(JwtSecurityToken jwtToken)
    {
        return jwtToken.Claims.Any() &&
               jwtToken.Claims.Any(x => x.Type == ClaimTypes.Sid && !string.IsNullOrEmpty(x.Value));
    }
}