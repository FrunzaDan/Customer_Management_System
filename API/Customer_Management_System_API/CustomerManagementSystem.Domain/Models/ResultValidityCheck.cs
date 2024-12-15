namespace CustomerManagementSystem.Domain.Models;

public sealed class ResultValidityCheck
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
}