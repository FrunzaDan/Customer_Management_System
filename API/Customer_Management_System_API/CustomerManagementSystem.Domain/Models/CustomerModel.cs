namespace CustomerManagementSystem.Domain.Models
{
    public class CustomerModel : ResponseModel
    {
        public string? Guid { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MSISDN { get; set; }
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
        public int searchOption { get; set; }
        public string? searchVariable { get; set; }
    }
}