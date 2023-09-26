using System;
using Customer_Management_System_Library.DataAccess;
using System.Net.Http;
using System.Security.Claims;

using Customer_Management_System_Library.Configuration;
using Customer_Management_System_Library.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace Customer_Management_System_Library.Auth
{
    public class JWTValidation
    {
        private string JWTKey;

        public readonly ICMSConfig _configuration;

        public JWTValidation(ICMSConfig configuration)
        {
            _configuration = configuration;
            JWTKey = _configuration.SecureJWTKey;
        }

        public bool Authorization(HttpContext httpContext, string? bearerToken)
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
            JWTValidation jwtAu = new JWTValidation(_configuration);
            Dictionary<string, object> callParam = new Dictionary<string, object>();

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

            if (jwtAu.ValidateToken(jwt))
            {
                return true;
            }
            return false;
        }



        public bool ValidateToken(string token)
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
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                JwtSecurityToken jwtToken = (JwtSecurityToken)validatedToken;

                var userId = jwtToken.Claims.First(x => x.Type == "id").Value;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public ClaimsPrincipal? GetPrincipal(string token)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                if (jwtToken == null)
                    return null;
                byte[] key = Convert.FromBase64String(JWTKey);
                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
                SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, parameters, out securityToken);
                return principal;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}

