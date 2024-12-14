using System.ComponentModel.DataAnnotations;

namespace CustomerManagementSystem.Domain.Models;

public class CustomerModel : ResponseModel
{
    public string? Guid { get; set; }

    [Required(ErrorMessage = "First Name is required!")]
    public string? FirstName { get; set; }

    [Required(ErrorMessage = "Last Name is required!")]
    public string? LastName { get; set; }

    [Required(ErrorMessage = "MSISDN is required!")]
    public string? Msisdn { get; set; }

    [Required(ErrorMessage = "Email is required!")]
    public string? Email { get; set; }

    public int? CustomerStatus { get; set; }

    public string? CreationDate { get; set; }

    public string? InteractionDate { get; set; }

    public int? Gender { get; set; }

    public string? Birthdate { get; set; }

    public AddressModel? Address { get; set; }
}

public class GetCustomerRequest
{
    public int SearchOption { get; set; }
    public string? SearchVariable { get; set; }
}