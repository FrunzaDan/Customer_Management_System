namespace Customer_Management_System_Library.Models
{
    public sealed class AccessTokenResponse : ResponseModel
    {

        public string? AccessToken { get; set; }

        public string? ValidUntil { get; set; }

    }
}

