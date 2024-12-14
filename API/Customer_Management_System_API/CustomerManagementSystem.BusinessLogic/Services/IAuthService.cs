using CustomerManagementSystem.Domain.Models;

namespace CustomerManagementSystem.BusinessLogic.Services;

public interface IAuthService
{
    public Task<AccessTokenResponse> GetAccessToken(MerchantCredentials merchantCredentials, HttpClient httpClient);

    public ResponseModel VerifyToken(string accessToken);
}