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

    public AccessTokenResponse GetAccessToken(MerchantCredentials merchantCredentials, HttpClient httpClient)
    {
        var accessTokenRsp = new AccessTokenResponse();
        try
        {
            var jwtCreation = new JwtCreation(_configuration, _dbUtils);

            if (merchantCredentials.MerchantId is not null && merchantCredentials.MerchantPassword is not null)
            {
                accessTokenRsp = jwtCreation.GenerateBearerJwt(merchantCredentials.MerchantId,
                    merchantCredentials.MerchantPassword);
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
            var jwtValidation = new JwtValidation(_configuration);
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