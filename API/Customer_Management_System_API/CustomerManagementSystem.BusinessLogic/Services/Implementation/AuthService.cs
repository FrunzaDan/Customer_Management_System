using System.Net.Http.Headers;
using CustomerManagementSystem.BusinessLogic.AuthFunctions;
using CustomerManagementSystem.BusinessLogic.Configuration;
using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace CustomerManagementSystem.BusinessLogic.Services.Implementation;

public class AuthService : IAuthService
{
    private readonly IBllConfig _configuration = ServiceLocator.GetServiceFromServiceProvider<IBllConfig>();
    private readonly IDbUtils _dbUtils = ServiceLocator.GetServiceFromServiceProvider<IDbUtils>();
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<AccessTokenResponse> GetAccessToken(MerchantCredentials merchantCredentials,
        HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(merchantCredentials);
        ArgumentNullException.ThrowIfNull(httpClient);

        var response = new AccessTokenResponse();

        try
        {
            var jwtCreation = new JwtCreation(_configuration, _dbUtils);
            if (string.IsNullOrEmpty(merchantCredentials.MerchantId) ||
                string.IsNullOrEmpty(merchantCredentials.MerchantPassword))
                return new AccessTokenResponse
                {
                    Status = StatusCodes.Status400BadRequest,
                    ResponseMessage = "Invalid Merchant Credentials"
                };

            response = await jwtCreation.GenerateBearerJwt(merchantCredentials.MerchantId,
                merchantCredentials.MerchantPassword);

            if (response.Status == StatusCodes.Status200OK && !string.IsNullOrEmpty(response.AccessToken))
            {
                if (VerifyToken(response.AccessToken).Status == StatusCodes.Status200OK)
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", response.AccessToken);
                }
                else
                {
                    response.AccessToken = null;
                    response.ValidUntil = null;
                    response.Status = StatusCodes.Status500InternalServerError;
                    response.ResponseMessage = "Token generated but could not be verified!";
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

    public ResponseModel VerifyToken(string accessToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
            throw new ArgumentNullException(nameof(httpContext));
        ArgumentNullException.ThrowIfNull(httpContext);
        if (string.IsNullOrEmpty(accessToken))
            throw new ArgumentNullException(nameof(accessToken));
        ArgumentNullException.ThrowIfNull(httpContext);

        var response = new ResponseModel();

        try
        {
            var jwtValidation = new JwtValidation(_configuration);
            var isAuthorized = jwtValidation.Authorize(httpContext, accessToken);

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