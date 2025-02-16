using CustomerManagementSystem.BusinessLogic.Services;
using CustomerManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CustomerManagementSystem.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController(ICustomerService customerService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> RegisterCustomer([FromBody] CustomerModel customerRqst)
    {
        try
        {
            var response = await customerService.RegisterCustomer(customerRqst);
            return StatusCode(response.Status ?? 200, response);
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { Message = "An error occurred while processing your request.", Details = ex.Message });
        }
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetCustomer([FromQuery] string searchVariable)
    {
        try
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
        catch (Exception ex)
        {
            return StatusCode(500,
                new { Message = "An error occurred while processing your request.", Details = ex.Message });
        }
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetCustomers()
    {
        try
        {
            var response = await customerService.GetCustomers();
            return StatusCode(response.Status ?? 200, response);
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { Message = "An error occurred while processing your request.", Details = ex.Message });
        }
    }

    [HttpPatch("edit")]
    public async Task<IActionResult> EditCustomer([FromBody] CustomerModel editCustomerRqst)
    {
        try
        {
            var response = await customerService.EditCustomer(editCustomerRqst);
            return StatusCode(response.Status ?? 200, response);
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { Message = "An error occurred while processing your request.", Details = ex.Message });
        }
    }

    [HttpPatch("deactivate")]
    public async Task<IActionResult> DeactivateCustomer([FromQuery] string customerGuid)
    {
        try
        {
            if (string.IsNullOrEmpty(customerGuid))
                return BadRequest(new { Message = "Customer GUID cannot be null or empty." });

            var response = await customerService.DeactivateCustomer(customerGuid);
            return StatusCode(response.Status ?? 200, response);
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { Message = "An error occurred while processing your request.", Details = ex.Message });
        }
    }
    
    [HttpPatch("reactivate")]
    public async Task<IActionResult> ReactivateCustomer([FromQuery] string customerGuid)
    {
        try
        {
            if (string.IsNullOrEmpty(customerGuid))
                return BadRequest(new { Message = "Customer GUID cannot be null or empty." });

            var response = await customerService.ReactivateCustomer(customerGuid);
            return StatusCode(response.Status ?? 200, response);
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { Message = "An error occurred while processing your request.", Details = ex.Message });
        }
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteCustomer([FromQuery] string customerGuid)
    {
        try
        {
            if (string.IsNullOrEmpty(customerGuid))
                return BadRequest(new { Message = "Customer GUID cannot be null or empty." });

            var response = await customerService.DeleteCustomer(customerGuid);
            return StatusCode(response.Status ?? 200, response);
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { Message = "An error occurred while processing your request.", Details = ex.Message });
        }
    }
}