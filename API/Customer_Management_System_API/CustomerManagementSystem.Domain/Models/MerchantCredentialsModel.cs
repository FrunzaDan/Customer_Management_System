using System.ComponentModel.DataAnnotations;

namespace CustomerManagementSystem.Domain.Models;

public sealed class MerchantCredentials
{
    [Required] public string? MerchantId { get; set; }

    [Required] public string? MerchantPassword { get; set; }
}