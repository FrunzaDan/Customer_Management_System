using CustomerManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace CustomerManagementSystem.BusinessLogic.Services
{
    public interface IAuthService
    {
        public AccessTokenResponse GetAccessToken(MerchantCredentials merchantCredentials, HttpClient httpClient);

        public AccessTokenResponse VerifyToken(string accessToken, HttpContext httpContext);
    }
}