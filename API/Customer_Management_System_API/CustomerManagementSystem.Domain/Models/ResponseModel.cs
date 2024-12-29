namespace CustomerManagementSystem.Domain.Models;

public class ResponseModel<T>
{
    public int? Status { get; set; }
    public string? ResponseMessage { get; set; }
    public T? Data { get; set; }

    // Constructor to initialize properties
    public ResponseModel(int? status = null, string? responseMessage = null, T? data = default)
    {
        Status = status;
        ResponseMessage = responseMessage;
        Data = data;
    }
}