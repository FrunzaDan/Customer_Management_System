using CustomerManagementSystem.BusinessLogic.Auth;
using CustomerManagementSystem.BusinessLogic.Configuration;
using CustomerManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace CustomerManagementSystem.BusinessLogic.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly ICMSConfig _configuration;

        public AuthService(ICMSConfig configuration)
        {
            _configuration = configuration;
        }

        public AccessTokenResponse GetAccessToken(MerchantCredentials merchantCredentials, HttpClient httpClient)
        {
            AccessTokenResponse accessTokenRsp = new AccessTokenResponse();
            try
            {
                JWTCreation jwtCreation = new JWTCreation(_configuration);

                if (merchantCredentials.merchantID is not null && merchantCredentials.merchantPassword is not null)
                {
                    accessTokenRsp = jwtCreation.GenerateBearerJWT(merchantCredentials.merchantID, merchantCredentials.merchantPassword);
                    if (accessTokenRsp.ResponseCode == StatusCodes.Status200OK && !string.IsNullOrEmpty(accessTokenRsp.AccessToken))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenRsp.AccessToken);
                    }
                }
            }
            catch (Exception ex)
            {
                accessTokenRsp.ResponseCode = StatusCodes.Status500InternalServerError;
                accessTokenRsp.ResponseMessage = ex.ToString();
            }
            return accessTokenRsp;
        }

        public AccessTokenResponse VerifyToken(string accessToken, HttpContext httpContext)
        {
            AccessTokenResponse verifyTokenRsp = new AccessTokenResponse();
            try
            {
                ResponseModel response = new ResponseModel();

                MerchantCredentials clientDetails = new MerchantCredentials();
                JWTValidation jwtValidation = new JWTValidation(_configuration);
                if (jwtValidation.Authorize(httpContext, accessToken))
                {
                    verifyTokenRsp.ResponseCode = StatusCodes.Status200OK;
                    verifyTokenRsp.ResponseMessage = "You Have Access Rights!";
                }
                else
                {
                    verifyTokenRsp.ResponseCode = StatusCodes.Status403Forbidden;
                    verifyTokenRsp.ResponseMessage = "No Access Rights!";
                }
            }
            catch (Exception ex)
            {
                verifyTokenRsp.ResponseCode = StatusCodes.Status500InternalServerError;
                verifyTokenRsp.ResponseMessage = ex.ToString();
            }
            return verifyTokenRsp;
        }
    }
}