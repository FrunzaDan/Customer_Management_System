using CustomerManagementSystem.BusinessLogic.Services;
using CustomerManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CustomerManagementSystem.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthenticationController(
        IHttpClientFactory httpClientFactory, 
        IAuthService authService)
    {
        _httpClientFactory = httpClientFactory;
        _authService = authService;
    }

    [Route("[action]")]
    [HttpPost]
    public async Task<AccessTokenResponse> GetAccessToken(MerchantCredentials merchantCredentials)
    {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await _authService.GetAccessToken(merchantCredentials, httpClient);
            if (response.ResponseCode.HasValue)
            {
                Response.StatusCode = (int)response.ResponseCode;
            }
            return response;
    }

    [Route("[action]")]
    [HttpGet]
    public ResponseModel VerifyToken(string accessToken)
    {
        var httpContext = HttpContext;
        var response = _authService.VerifyToken(accessToken, httpContext);

        if (response.ResponseCode.HasValue)
        {
            Response.StatusCode = (int)response.ResponseCode;
        }
        return response;
    }
}