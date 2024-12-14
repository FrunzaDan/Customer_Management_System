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
        if (response.Status.HasValue) Response.StatusCode = (int)response.Status;

        return response;
    }

    [Route("[action]")]
    [HttpGet]
    public ResponseModel VerifyToken(string accessToken)
    {
        var response = _authService.VerifyToken(accessToken);

        if (response.Status.HasValue) Response.StatusCode = (int)response.Status;

        return response;
    }
}