namespace CustomerManagementSystem.Domain.Models;

public class AuditLogEntry
{
    public int AuditId { get; set; }

    public string? CustomerGuid { get; set; }

    public string? MerchantId { get; set; }

    public string? Action { get; set; }

    public string? Details { get; set; }

    public DateTime ActionDate { get; set; }
}
