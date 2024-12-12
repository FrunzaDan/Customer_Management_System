namespace CustomerManagementSystem.Domain.Models;

public class CustomerListModel : ResponseModel
{
    public List<CustomerModel>? CustomerList { get; set; }
}