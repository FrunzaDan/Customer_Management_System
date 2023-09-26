using System.Net.Http.Headers;
using Customer_Management_System_Library.Models;
using Customer_Management_System_Library.Configuration;
using Customer_Management_System_Library.Auth;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using Azure;

namespace Customer_Management_System_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ICMSConfig _configuration;

        private readonly IHttpClientFactory _httpClientFactory;

        public AuthenticationController(ICMSConfig configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        [Route("[action]")]
        [HttpPost]
        public AccessTokenResponse GetAccessToken(MerchantCredentials merchantCredentials)
        {
            AccessTokenResponse accessTokenRsp = new AccessTokenResponse();
            try
            {
                JWTCreation jwtCreation = new(_configuration);

                accessTokenRsp = jwtCreation.GenerateBearerJWT(merchantCredentials.MerchantID, merchantCredentials.MerchantPassword);
                if (accessTokenRsp.ResponseCode == StatusCodes.Status200OK && !string.IsNullOrEmpty(accessTokenRsp.AccessToken))
                {
                    var httpClient = _httpClientFactory.CreateClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenRsp.AccessToken);
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
        [HttpPost]
        public AccessTokenResponse MerchantLogin(MerchantCredentials merchantCredentials)
        {
            AccessTokenResponse accessTokenRsp = new AccessTokenResponse();
            try
            {
                JWTCreation jwtCreation = new(_configuration);
                if (merchantCredentials.MerchantID is not null && merchantCredentials.MerchantPassword is not null)
                {
                    accessTokenRsp = jwtCreation.GenerateBearerJWT(merchantCredentials.MerchantID, merchantCredentials.MerchantPassword);
                }

                ResponseModel response = new ResponseModel();
                HttpContext httpContext = HttpContext;
                MerchantCredentials clientDetails = new MerchantCredentials();
                JWTValidation jwtValidation = new JWTValidation(_configuration);
                if (jwtValidation.Authorization(httpContext, accessTokenRsp.AccessToken))
                {
                    accessTokenRsp.ResponseCode = StatusCodes.Status200OK;
                    accessTokenRsp.ResponseMessage = "You Have Access Rights!";
                }
                else
                {
                    accessTokenRsp.ResponseCode = StatusCodes.Status403Forbidden;
                    accessTokenRsp.ResponseMessage = "No Access Rights!";
                }
            }
            catch (Exception ex)
            {
                accessTokenRsp.ResponseCode = StatusCodes.Status500InternalServerError;
                accessTokenRsp.ResponseMessage = ex.ToString();
            }
            Response.StatusCode = (int)accessTokenRsp.ResponseCode;
            return accessTokenRsp;
        }
    }
}