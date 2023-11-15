using System.Security.Claims;
using Customer_Management_System_Library.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace Customer_Management_System_Library.Auth
{
    public class JWTValidation
    {
        private string JWTKey;
        private string JWTIssuer;
        private string JWTAudience;
        public readonly ICMSConfig _configuration;
        public JWTValidation(ICMSConfig configuration)
        {
            _configuration = configuration;
            JWTKey = _configuration.SecureJWTKey;
            JWTIssuer = _configuration.JWTIssuer;
            JWTAudience = _configuration.JWTAudience;
        }
        public bool Authorize(HttpContext httpContext, string? bearerToken)
        {
            string authHeader = String.Empty;
            if (String.IsNullOrEmpty(bearerToken))
            {
                authHeader = httpContext.Request.Headers["Authorization"].ToString() ?? "Error";
            }
            else
            {
                authHeader = "Bearer " + bearerToken;
            }
            JWTValidation jwtValidation = new JWTValidation(_configuration);

            if (string.IsNullOrEmpty(authHeader))
            {
                return false;
            }

            if (!authHeader.StartsWith("Bearer "))
            {
                return false;
            }

            string jwt = authHeader.Split(' ')[1];
            if (string.IsNullOrEmpty(jwt))
            {
                throw new SecurityTokenException("Missing JWT Token in Authorization HTTP Header");
            }

            if (jwtValidation.ValidateToken(jwt))
            {
                return true;
            }
            return false;
        }

        private bool ValidateToken(string token)
        {
            if (token == null)
            {
                return false;
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = System.Text.Encoding.ASCII.GetBytes(JWTKey);
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
                }, out SecurityToken validatedToken);

                JwtSecurityToken jwtToken = (JwtSecurityToken)validatedToken;

                bool claimsAreValid = VerifyClaims(jwtToken);

                if (claimsAreValid)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                return false;
            }
        }

        private bool VerifyClaims(JwtSecurityToken jwtToken)
        {
            bool claimsAreValid = false;

            var userId = jwtToken.Claims.First(x => x.Type == ClaimTypes.Sid).Value;

            if (!string.IsNullOrEmpty(userId))
            {
                claimsAreValid = true;
            }
            return claimsAreValid;
        }
    }
}

