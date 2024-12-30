namespace CustomerManagementSystem.Domain.Models;

public class ResponseModel<T>
{
    // Constructor to initialize properties
    public ResponseModel(int? status = null, string? responseMessage = null, T? data = default)
    {
        Status = status;
        ResponseMessage = responseMessage;
        Data = data;
    }

    public int? Status { get; set; }
    public string? ResponseMessage { get; set; }
    public T? Data { get; set; }
}