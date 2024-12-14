using System.ComponentModel.DataAnnotations;

namespace CustomerManagementSystem.Domain.Models;

public sealed class MerchantCredentialsCheck
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
}