using System.Security.Claims;
using Customer_Management_System_Library.Configuration;
using Customer_Management_System_Library.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Customer_Management_System_Library.DataAccess;
using NuGet.Protocol.Plugins;

namespace Customer_Management_System_Library.Auth
{
    public class JWTCreation
    {
        private string JWTKey;

        public readonly ICMSConfig _configuration;

        public JWTCreation(ICMSConfig configuration)
        {
            _configuration = configuration;
            JWTKey = _configuration.SecureJWTKey;
        }

        public AccessTokenResponse GenerateBearerJWT(string merchantID, string merchantPassword)
        {
            AccessTokenResponse accessTokenRsp = new AccessTokenResponse();

            MerchantCredentials merchantCredentials = new MerchantCredentials();

            merchantCredentials.MerchantID = merchantID;
            merchantCredentials.MerchantPassword = merchantPassword;

            var dbUtils = new DBUtils(_configuration);
            bool isValid = dbUtils.CheckMerchantCredentialsFromDB(merchantCredentials);

            if (isValid)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = System.Text.Encoding.ASCII.GetBytes(JWTKey);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] { new Claim("id", merchantID) }),
                    Expires = DateTime.UtcNow.AddMinutes(Double.Parse(_configuration.AccessTokenTimeout)),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var accessToken = tokenHandler.WriteToken(token);
                accessTokenRsp.AccessToken = accessToken;
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

    }
}

