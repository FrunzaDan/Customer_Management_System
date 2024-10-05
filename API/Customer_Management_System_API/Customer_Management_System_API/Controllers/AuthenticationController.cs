using CustomerManagementSystem.BusinessLogic.Services;
using CustomerManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Customer_Management_System_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly IAuthService _authService;

        public AuthenticationController(IHttpClientFactory httpClientFactory, IAuthService authService)
        {
            _httpClientFactory = httpClientFactory;
            _authService = authService;
        }

        [Route("[action]")]
        [HttpPost]
        public AccessTokenResponse GetAccessToken(MerchantCredentials merchantCredentials)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var accessTokenRsp = _authService.GetAccessToken(merchantCredentials, httpClient);
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
            var httpContext = HttpContext;
            var verifyTokenRsp = _authService.VerifyToken(accessToken, httpContext);

            if (verifyTokenRsp.ResponseCode is not null)
            {
                Response.StatusCode = (int)verifyTokenRsp.ResponseCode;
            }
            return verifyTokenRsp;
        }
    }
}