using CustomerManagementSystem.BusinessLogic.Services;
using CustomerManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace CustomerManagementSystem.WebAPI.Controllers
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
        public IActionResult GetAccessToken([FromBody] MerchantCredentials merchantCredentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var accessTokenRsp = _authService.GetAccessToken(merchantCredentials, httpClient);
                if (accessTokenRsp.ResponseCode.HasValue)
                {
                    Response.StatusCode = (int)accessTokenRsp.ResponseCode;
                }

                return Ok(accessTokenRsp);
            }
            catch (HttpRequestException httpEx)
            {
                // Log error (use a logger service if needed)
                return StatusCode(500, $"Error making HTTP request: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                // Log error
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
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