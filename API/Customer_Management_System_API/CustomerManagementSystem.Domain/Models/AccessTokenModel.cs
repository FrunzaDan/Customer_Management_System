namespace CustomerManagementSystem.Domain.Models
{
    public sealed class AccessTokenResponse : ResponseModel
    {
        public string? AccessToken { get; set; }

        public string? ValidUntil { get; set; }
    }
}