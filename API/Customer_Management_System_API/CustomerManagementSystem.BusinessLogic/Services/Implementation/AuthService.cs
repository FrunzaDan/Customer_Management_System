using System.Net.Http.Headers;
using CustomerManagementSystem.BusinessLogic.AuthFunctions;
using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Configuration;
using CustomerManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace CustomerManagementSystem.BusinessLogic.Services.Implementation;

public class AuthService(IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory, IAppSettingsConfig appSettingsConfig, IDbUtils dbUtils)
    : IAuthService
{
    public async Task<ResponseModel<AccessTokenResponse>> GetAccessToken(MerchantCredentials merchantCredentials)
    {
        if (string.IsNullOrEmpty(merchantCredentials.MerchantId) ||
            string.IsNullOrEmpty(merchantCredentials.MerchantPassword))
            return new ResponseModel<AccessTokenResponse>
            {
                Status = StatusCodes.Status400BadRequest,
                ResponseMessage = "Invalid Merchant Credentials"
            };
        var httpClient = httpClientFactory.CreateClient();

        var response = new ResponseModel<AccessTokenResponse>();

        try
        {
            var jwtCreation = new JwtCreation(appSettingsConfig, dbUtils);
            response = await jwtCreation.GenerateBearerJwt(merchantCredentials.MerchantId,
                merchantCredentials.MerchantPassword);

            if (response is { Status: StatusCodes.Status200OK, Data: not null } &&
                !string.IsNullOrEmpty(response.Data.AccessToken))
            {
                if (VerifyToken(response.Data.AccessToken).Status == StatusCodes.Status200OK)
                {
                    httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", response.Data.AccessToken);
                }
                else
                {
                    response.Status = StatusCodes.Status500InternalServerError;
                    response.ResponseMessage = "Token generated but could not be verified!";
                    response.Data = null;
                }
            }
        }
        catch (Exception ex)
        {
            response.Status = StatusCodes.Status500InternalServerError;
            response.ResponseMessage = "An error occurred on our side while generating the access token: " + ex.Message;
        }

        return response;
    }

    public ResponseModel<object> VerifyToken(string accessToken)
    {
        var response = new ResponseModel<object>();
        if (string.IsNullOrEmpty(accessToken))
        {
            response.Status = StatusCodes.Status500InternalServerError;
            response.ResponseMessage = "No Access Token provided!";
        }

        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            response.Status = StatusCodes.Status500InternalServerError;
            response.ResponseMessage = "Failed to initialize the http context!";
        }

        try
        {
            var jwtValidation = new JwtValidation(appSettingsConfig);
            var isAuthorized = httpContext != null && jwtValidation.Authorize(httpContext, accessToken);

            response.Status = isAuthorized
                ? StatusCodes.Status200OK
                : StatusCodes.Status403Forbidden;

            response.ResponseMessage = isAuthorized
                ? "You Have Access Rights!"
                : "No Access Rights!";
        }
        catch (Exception ex)
        {
            response.Status = StatusCodes.Status500InternalServerError;
            response.ResponseMessage = "An error occurred while verifying the token: " + ex.Message;
        }

        return response;
    }
}