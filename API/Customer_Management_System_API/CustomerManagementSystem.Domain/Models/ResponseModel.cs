namespace CustomerManagementSystem.Domain.Models;

public class ResponseModel<T>
{
    public int? Status { get; set; }
    public string? ResponseMessage { get; set; }
    public T? Data { get; set; }
}