using System.Net.Http.Headers;
using CustomerManagementSystem.BusinessLogic.AuthFunctions;
using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Configuration;
using CustomerManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace CustomerManagementSystem.BusinessLogic.Services.Implementation;

public class AuthService(
    IHttpContextAccessor httpContextAccessor,
    IHttpClientFactory httpClientFactory,
    IAppSettingsConfig appSettingsConfig,
    IDbUtils dbUtils)
    : IAuthService
{
    public async Task<ResponseModel<AccessTokenResponse>> GetAccessToken(MerchantCredentials merchantCredentials)
    {
        if (string.IsNullOrWhiteSpace(merchantCredentials.MerchantId) ||
            string.IsNullOrWhiteSpace(merchantCredentials.MerchantPassword))
            return new ResponseModel<AccessTokenResponse>(403, "Invalid or empty merchant credentials.");

        ResponseModel<AccessTokenResponse> response;

        try
        {
            var jwtCreation = new JwtCreation(appSettingsConfig, dbUtils);
            response = await jwtCreation.GenerateBearerJwt(merchantCredentials.MerchantId,
                merchantCredentials.MerchantPassword);

            if (response is { Status: StatusCodes.Status200OK, Data: { AccessToken: { Length: > 0 } } })
            {
                var tokenVerification = VerifyToken(response.Data.AccessToken);
                if (tokenVerification.Status == StatusCodes.Status200OK)
                {
                    var httpClient = httpClientFactory.CreateClient();
                    httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", response.Data.AccessToken);
                }
                else
                {
                    return new ResponseModel<AccessTokenResponse>(
                        StatusCodes.Status500InternalServerError,
                        tokenVerification.ResponseMessage
                    );
                }
            }
        }
        catch (Exception ex)
        {
            return new ResponseModel<AccessTokenResponse>(
                StatusCodes.Status500InternalServerError,
                $"An error occurred while generating the access token: {ex.Message}"
            );
        }

        return response;
    }

    public ResponseModel<object> VerifyToken(string accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken)) return new ResponseModel<object>(500, "No Access Token provided!");

        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null) return new ResponseModel<object>(500, "Failed to initialize the HTTP context!");

        try
        {
            var jwtValidation = new JwtValidation(appSettingsConfig);
            var isAuthorized = jwtValidation.Authorize(httpContext, accessToken);

            return isAuthorized;
        }
        catch (Exception ex)
        {
            return new ResponseModel<object>(StatusCodes.Status500InternalServerError,
                $"An error occurred while verifying the access token: {ex.Message}"
            );
        }
    }
}