using System.Net.Http.Headers;
using CustomerManagementSystem.BusinessLogic.AuthFunctions;
using CustomerManagementSystem.BusinessLogic.Configuration;
using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace CustomerManagementSystem.BusinessLogic.Services.Implementation;

public class AuthService : IAuthService
{
    private readonly IBLLConfig _configuration;
    private readonly IDBUtils _dbUtils;

    public AuthService()
    {
        _configuration = ServiceLocator.GetService<IBLLConfig>();
        _dbUtils = ServiceLocator.GetService<IDBUtils>();
    }

    public AccessTokenResponse GetAccessToken(MerchantCredentials merchantCredentials, HttpClient httpClient)
    {
        var accessTokenRsp = new AccessTokenResponse();
        try
        {
            var jwtCreation = new JwtCreation(_configuration, _dbUtils);

            if (merchantCredentials.merchantID is not null && merchantCredentials.merchantPassword is not null)
            {
                accessTokenRsp = jwtCreation.GenerateBearerJwt(merchantCredentials.merchantID,
                    merchantCredentials.merchantPassword);
                if (accessTokenRsp.ResponseCode == StatusCodes.Status200OK &&
                    !string.IsNullOrEmpty(accessTokenRsp.AccessToken))
                    httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", accessTokenRsp.AccessToken);
            }
        }
        catch (Exception ex)
        {
            accessTokenRsp.ResponseCode = StatusCodes.Status500InternalServerError;
            accessTokenRsp.ResponseMessage = ex.ToString();
        }

        return accessTokenRsp;
    }

    public AccessTokenResponse VerifyToken(string accessToken, HttpContext httpContext)
    {
        var verifyTokenRsp = new AccessTokenResponse();
        try
        {
            var response = new ResponseModel();

            var clientDetails = new MerchantCredentials();
            var jwtValidation = new JWTValidation(_configuration);
            if (jwtValidation.Authorize(httpContext, accessToken))
            {
                verifyTokenRsp.ResponseCode = StatusCodes.Status200OK;
                verifyTokenRsp.ResponseMessage = "You Have Access Rights!";
            }
            else
            {
                verifyTokenRsp.ResponseCode = StatusCodes.Status403Forbidden;
                verifyTokenRsp.ResponseMessage = "No Access Rights!";
            }
        }
        catch (Exception ex)
        {
            verifyTokenRsp.ResponseCode = StatusCodes.Status500InternalServerError;
            verifyTokenRsp.ResponseMessage = ex.ToString();
        }

        return verifyTokenRsp;
    }
}