using System.Net.Http.Headers;
using CustomerManagementSystem.BusinessLogic.AuthFunctions;
using CustomerManagementSystem.BusinessLogic.Configuration;
using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace CustomerManagementSystem.BusinessLogic.Services.Implementation;

public class AuthService : IAuthService
{
    private readonly IBllConfig _configuration = ServiceLocator.GetService<IBllConfig>();
    private readonly IDbUtils _dbUtils = ServiceLocator.GetService<IDbUtils>();

    public async Task<AccessTokenResponse> GetAccessToken(MerchantCredentials merchantCredentials, HttpClient httpClient)
    {
        
        ArgumentNullException.ThrowIfNull(merchantCredentials);
        ArgumentNullException.ThrowIfNull(httpClient);

        var accessTokenResponse = new AccessTokenResponse();

        try
        {
            var jwtCreation = new JwtCreation(_configuration, _dbUtils);
            if (string.IsNullOrEmpty(merchantCredentials.MerchantId) ||
                string.IsNullOrEmpty(merchantCredentials.MerchantPassword))
            {
                return new AccessTokenResponse
                {
                    ResponseCode = StatusCodes.Status400BadRequest,
                    ResponseMessage = "Invalid Merchant Credentials"
                };
            }

            accessTokenResponse = await jwtCreation.GenerateBearerJwt(merchantCredentials.MerchantId, merchantCredentials.MerchantPassword);

            if (accessTokenResponse.ResponseCode == StatusCodes.Status200OK && 
                !string.IsNullOrEmpty(accessTokenResponse.AccessToken))
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessTokenResponse.AccessToken);
            }
        }
        catch (Exception ex)
        {
            accessTokenResponse.ResponseCode = StatusCodes.Status500InternalServerError;
            accessTokenResponse.ResponseMessage = "An error occurred on our side while generating the access token.";
        }

        return accessTokenResponse;
    }

    public ResponseModel VerifyToken(string accessToken, HttpContext httpContext)
    {
        if (string.IsNullOrEmpty(accessToken))
            throw new ArgumentNullException(nameof(accessToken));
        ArgumentNullException.ThrowIfNull(httpContext);

        var verifyTokenResponse = new AccessTokenResponse();

        try
        {
            var jwtValidation = new JwtValidation(_configuration);
            var isAuthorized = jwtValidation.Authorize(httpContext, accessToken);

            verifyTokenResponse.ResponseCode = isAuthorized
                ? StatusCodes.Status200OK
                : StatusCodes.Status403Forbidden;

            verifyTokenResponse.ResponseMessage = isAuthorized
                ? "You Have Access Rights!"
                : "No Access Rights!";
        }
        catch (Exception ex)
        {
            verifyTokenResponse.ResponseCode = StatusCodes.Status500InternalServerError;
            verifyTokenResponse.ResponseMessage = "An error occurred while verifying the token.";
        }

        return verifyTokenResponse;
    }
}