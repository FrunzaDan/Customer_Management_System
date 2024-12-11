using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CustomerManagementSystem.BusinessLogic.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace CustomerManagementSystem.BusinessLogic.AuthFunctions;

public class JWTValidation
{
    public readonly IBLLConfig _configuration;
    private readonly string JWTAudience;
    private readonly string JWTIssuer;
    private readonly string JWTKey;

    public JWTValidation(IBLLConfig configuration)
    {
        _configuration = configuration;
        JWTKey = _configuration.SecureJWTKey;
        JWTIssuer = _configuration.JWTIssuer;
        JWTAudience = _configuration.JWTAudience;
    }

    public bool Authorize(HttpContext httpContext, string? bearerToken)
    {
        var authHeader = string.Empty;
        if (string.IsNullOrEmpty(bearerToken))
            authHeader = httpContext.Request.Headers["Authorization"].ToString() ?? "Error";
        else
            authHeader = "Bearer " + bearerToken;
        var jwtValidation = new JWTValidation(_configuration);

        if (string.IsNullOrEmpty(authHeader)) return false;

        if (!authHeader.StartsWith("Bearer ")) return false;

        var jwt = authHeader.Split(' ')[1];
        if (string.IsNullOrEmpty(jwt))
            throw new SecurityTokenException("Missing JWT Token in Authorization HTTP Header");

        if (jwtValidation.ValidateToken(jwt)) return true;
        return false;
    }

    private bool ValidateToken(string token)
    {
        if (token == null) return false;
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(JWTKey);
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = JWTIssuer,
                ValidateAudience = true,
                ValidAudience = JWTAudience,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            var claimsAreValid = VerifyClaims(jwtToken);

            if (claimsAreValid)
                return true;
            return false;
        }
        catch (Exception ex)
        {
            ex.ToString();
            return false;
        }
    }

    private bool VerifyClaims(JwtSecurityToken jwtToken)
    {
        var claimsAreValid = false;

        var userId = jwtToken.Claims.First(x => x.Type == ClaimTypes.Sid).Value;

        if (!string.IsNullOrEmpty(userId)) claimsAreValid = true;
        return claimsAreValid;
    }
}