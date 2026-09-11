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
    [HttpPost("register")]
    public async Task<IActionResult> RegisterCustomer([FromBody] CustomerModel customerRqst)
    {
        var response = await customerService.RegisterCustomer(customerRqst);
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
    public async Task<IActionResult> GetCustomers()
    {
        var response = await customerService.GetCustomers();
        return StatusCode(response.Status ?? 200, response);
    }

    [HttpPatch("edit")]
    public async Task<IActionResult> EditCustomer([FromBody] CustomerModel editCustomerRqst)
    {
        var response = await customerService.EditCustomer(editCustomerRqst);
        return StatusCode(response.Status ?? 200, response);
    }

    [HttpPatch("deactivate")]
    public async Task<IActionResult> DeactivateCustomer([FromQuery] string customerGuid)
    {
        if (string.IsNullOrEmpty(customerGuid))
            return BadRequest(new { Message = "Customer GUID cannot be null or empty." });

        var response = await customerService.DeactivateCustomer(customerGuid);
        return StatusCode(response.Status ?? 200, response);
    }

    [HttpPatch("reactivate")]
    public async Task<IActionResult> ReactivateCustomer([FromQuery] string customerGuid)
    {
        if (string.IsNullOrEmpty(customerGuid))
            return BadRequest(new { Message = "Customer GUID cannot be null or empty." });

        var response = await customerService.ReactivateCustomer(customerGuid);
        return StatusCode(response.Status ?? 200, response);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteCustomer([FromQuery] string customerGuid)
    {
        if (string.IsNullOrEmpty(customerGuid))
            return BadRequest(new { Message = "Customer GUID cannot be null or empty." });

        var response = await customerService.DeleteCustomer(customerGuid);
        return StatusCode(response.Status ?? 200, response);
    }
}
