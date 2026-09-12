using CustomerManagementSystem.BusinessLogic.Services;
using CustomerManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerManagementSystem.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomerController(ICustomerService customerService) : ControllerBase
{
    // Every [Authorize]-gated request has a verified JWT with ClaimTypes.Name set to the
    // merchant ID (see JwtCreation.BuildTokenDescriptor) — never null/empty in practice.
    private string MerchantId => User.Identity!.Name!;

    [HttpPost("register")]
    public async Task<IActionResult> RegisterCustomer([FromBody] CustomerModel customerRqst)
    {
        var response = await customerService.RegisterCustomer(customerRqst, MerchantId);
        return StatusCode(response.Status ?? 200, response);
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetCustomer([FromQuery] string searchVariable)
    {
        if (string.IsNullOrEmpty(searchVariable))
            return BadRequest(new { Message = "Search variable cannot be null or empty." });

        var getCustomerRqst = new GetCustomerRequest
        {
            SearchVariable = searchVariable
        };
        var response = await customerService.GetCustomer(getCustomerRqst);
        return StatusCode(response.Status ?? 200, response);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetCustomers([FromQuery] GetCustomersRequest request)
    {
        var response = await customerService.GetCustomers(request);
        return StatusCode(response.Status ?? 200, response);
    }

    [HttpGet("auditLog")]
    public async Task<IActionResult> GetCustomerAuditLog([FromQuery] string customerGuid)
    {
        if (string.IsNullOrEmpty(customerGuid))
            return BadRequest(new { Message = "Customer GUID cannot be null or empty." });

        var response = await customerService.GetCustomerAuditLog(customerGuid);
        return StatusCode(response.Status ?? 200, response);
    }

    [HttpPatch("edit")]
    public async Task<IActionResult> EditCustomer([FromBody] CustomerModel editCustomerRqst)
    {
        var response = await customerService.EditCustomer(editCustomerRqst, MerchantId);
        return StatusCode(response.Status ?? 200, response);
    }

    [HttpPatch("deactivate")]
    public async Task<IActionResult> DeactivateCustomer([FromQuery] string customerGuid)
    {
        if (string.IsNullOrEmpty(customerGuid))
            return BadRequest(new { Message = "Customer GUID cannot be null or empty." });

        var response = await customerService.DeactivateCustomer(customerGuid, MerchantId);
        return StatusCode(response.Status ?? 200, response);
    }

    [HttpPatch("reactivate")]
    public async Task<IActionResult> ReactivateCustomer([FromQuery] string customerGuid)
    {
        if (string.IsNullOrEmpty(customerGuid))
            return BadRequest(new { Message = "Customer GUID cannot be null or empty." });

        var response = await customerService.ReactivateCustomer(customerGuid, MerchantId);
        return StatusCode(response.Status ?? 200, response);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteCustomer([FromQuery] string customerGuid)
    {
        if (string.IsNullOrEmpty(customerGuid))
            return BadRequest(new { Message = "Customer GUID cannot be null or empty." });

        var response = await customerService.DeleteCustomer(customerGuid, MerchantId);
        return StatusCode(response.Status ?? 200, response);
    }
}
