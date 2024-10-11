using System.ComponentModel.DataAnnotations;

namespace CustomerManagementSystem.Domain.Models
{
    public sealed class MerchantCredentials
    {
        [Required]
        public string? merchantID { get; set; }

        [Required]
        public string? merchantPassword { get; set; }
    }
}