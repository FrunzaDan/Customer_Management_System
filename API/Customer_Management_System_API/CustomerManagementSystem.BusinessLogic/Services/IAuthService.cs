using CustomerManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace CustomerManagementSystem.BusinessLogic.Services;

public interface IAuthService
{
    public Task<AccessTokenResponse> GetAccessToken(MerchantCredentials merchantCredentials, HttpClient httpClient);

    public ResponseModel VerifyToken(string accessToken, HttpContext httpContext);
}