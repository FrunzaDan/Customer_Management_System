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
    public async Task<ResponseModel<object>> GetAccessToken(MerchantCredentials merchantCredentials)
    {
        if (string.IsNullOrWhiteSpace(merchantCredentials.MerchantId) ||
            string.IsNullOrWhiteSpace(merchantCredentials.MerchantPassword))
            return new ResponseModel<object>(403, "Invalid or empty merchant credentials.");

        var jwtCreation = new JwtCreation(appSettingsConfig, dbUtils);
        var response = await jwtCreation.GenerateBearerJwt(merchantCredentials);

        if (response is not { Status: 200, Data: AccessTokenResponse accessTokenResponse }) return response;

        var tokenVerification = VerifyToken(accessTokenResponse.AccessToken);

        if (tokenVerification.Status != StatusCodes.Status200OK)
            return new ResponseModel<object>(
                StatusCodes.Status500InternalServerError,
                tokenVerification.ResponseMessage
            );
        
        var httpClient = httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessTokenResponse.AccessToken);


        return response;
    }

    public ResponseModel<object> VerifyToken(string? accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken)) return new ResponseModel<object>(500, "No Access Token provided!");

        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null) return new ResponseModel<object>(500, "Failed to initialize the HTTP context!");

        var jwtValidation = new JwtValidation(appSettingsConfig);
        var isAuthorized = jwtValidation.Authorize(httpContext, accessToken);

        return isAuthorized;
    }
}