namespace Customer_Management_System_Library.Models
{
    public class CustomerListModel : ResponseModel
    {
        public List<CustomerModel>? customerList { get; set; }
    }
}