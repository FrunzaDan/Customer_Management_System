using CustomerManagementSystem.BusinessLogic.Services;
using CustomerManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CustomerManagementSystem.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController(IAuthService authService) : ControllerBase
{
    [HttpPost("access-token")]
    public async Task<IActionResult> GetAccessToken([FromBody] MerchantCredentials merchantCredentials)
    {
        try
        {
            var response = await authService.GetAccessToken(merchantCredentials);
            return StatusCode(response.Status ?? 200, response);
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { Message = "An error occurred while processing your request.", Details = ex.Message });
        }
    }

    [HttpGet("verify-token")]
    public ActionResult<ResponseModel<object>> VerifyToken([FromQuery] string? accessToken)
    {
        if (string.IsNullOrEmpty(accessToken))
            return BadRequest(new { Message = "Access token cannot be null or empty." });

        try
        {
            var response = authService.VerifyToken(accessToken);
            return StatusCode(response.Status ?? 200, response);
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { Message = "An error occurred while processing your request.", Details = ex.Message });
        }
    }
}