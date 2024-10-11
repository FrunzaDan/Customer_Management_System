using CustomerManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace CustomerManagementSystem.BusinessLogic.Services
{
    public interface ICustomerService
    {
        public AccessTokenResponse GetAccessToken(MerchantCredentials merchantCredentials, HttpClient httpClient);

        public AccessTokenResponse VerifyToken(string accessToken, HttpContext httpContext);
    }
}