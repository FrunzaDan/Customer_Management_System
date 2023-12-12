using Customer_Management_System_Library;
using Customer_Management_System_Library.Auth;
using Customer_Management_System_Library.Configuration;
using Customer_Management_System_Library.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace Customer_Management_System_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ICMSConfig _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IDBUtils _dbUtils;

        public AuthenticationController(ICMSConfig configuration, IHttpClientFactory httpClientFactory, IDBUtils dbUtils)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _dbUtils = dbUtils;
        }

        [Route("[action]")]
        [HttpPost]
        public AccessTokenResponse GetAccessToken(MerchantCredentials merchantCredentials)
        {
            AccessTokenResponse accessTokenRsp = new AccessTokenResponse();
            try
            {
                JWTCreation jwtCreation = new(_configuration, _dbUtils);

                if (merchantCredentials.merchantID is not null && merchantCredentials.merchantPassword is not null)
                {
                    accessTokenRsp = jwtCreation.GenerateBearerJWT(merchantCredentials.merchantID, merchantCredentials.merchantPassword);
                    if (accessTokenRsp.ResponseCode == StatusCodes.Status200OK && !string.IsNullOrEmpty(accessTokenRsp.AccessToken))
                    {
                        var httpClient = _httpClientFactory.CreateClient();
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenRsp.AccessToken);
                    }
                }
            }
            catch (Exception ex)
            {
                accessTokenRsp.ResponseCode = StatusCodes.Status500InternalServerError;
                accessTokenRsp.ResponseMessage = ex.ToString();
            }
            if (accessTokenRsp.ResponseCode is not null)
            {
                Response.StatusCode = (int)accessTokenRsp.ResponseCode;
            }
            return accessTokenRsp;
        }

        [Route("[action]")]
        [HttpGet]
        public AccessTokenResponse VerifyToken(string accessToken)
        {
            AccessTokenResponse verifyTokenRsp = new AccessTokenResponse();
            try
            {
                ResponseModel response = new ResponseModel();
                HttpContext httpContext = HttpContext;
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
            Response.StatusCode = (int)verifyTokenRsp.ResponseCode;
            return verifyTokenRsp;
        }
    }
}