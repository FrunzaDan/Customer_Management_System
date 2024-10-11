using CustomerManagementSystem.BusinessLogic.Configuration;
using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CustomerManagementSystem.BusinessLogic.AuthFunctions
{
    public class JWTCreation
    {
        private string JWTKey;
        private string JWTIssuer;
        private string JWTAudience;
        private readonly IBLLConfig _configuration;
        private readonly IDBUtils _dbUtils;

        public JWTCreation(IBLLConfig configuration, IDBUtils dbUtils)
        {
            _configuration = configuration;
            JWTKey = _configuration.SecureJWTKey;
            JWTIssuer = _configuration.JWTIssuer;
            JWTAudience = _configuration.JWTAudience;

            _dbUtils = dbUtils;
        }

        public AccessTokenResponse GenerateBearerJWT(string merchantID, string merchantPassword)
        {
            AccessTokenResponse accessTokenRsp = new AccessTokenResponse();
            MerchantCredentials merchantCredentials = new MerchantCredentials();

            merchantCredentials.merchantID = merchantID;
            merchantCredentials.merchantPassword = merchantPassword;

            bool credentialsAreValid = _dbUtils.CheckMerchantCredentialsFromDB(merchantCredentials);

            if (credentialsAreValid)
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                var tokenDescriptor = AddClaims(merchantID);

                var accessToken = tokenHandler.CreateToken(tokenDescriptor);
                var accessTokenString = tokenHandler.WriteToken(accessToken);
                accessTokenRsp.AccessToken = accessTokenString;
                accessTokenRsp.ValidUntil = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration.AccessTokenTimeout)).ToString();
                accessTokenRsp.ResponseCode = StatusCodes.Status200OK;
                accessTokenRsp.ResponseMessage = "Success!";
            }
            else
            {
                accessTokenRsp.AccessToken = null;
                accessTokenRsp.ValidUntil = null;
                accessTokenRsp.ResponseCode = StatusCodes.Status403Forbidden;
                accessTokenRsp.ResponseMessage = "No Access Rights!";
            }

            return accessTokenRsp;
        }

        private SecurityTokenDescriptor AddClaims(string merchantID)
        {
            var key = System.Text.Encoding.ASCII.GetBytes(JWTKey);
            var claims = new List<Claim> {
                    new(ClaimTypes.Sid, merchantID)
                };
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Double.Parse(_configuration.AccessTokenTimeout)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = JWTIssuer,
                Audience = JWTAudience
            };

            return tokenDescriptor;
        }
    }
}