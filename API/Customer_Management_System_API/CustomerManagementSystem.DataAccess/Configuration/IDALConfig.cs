namespace CustomerManagementSystem.BusinessLogic.Configuration
{
    public interface IDALConfig
    {
        string CustomerManagementSystemDB_Windows { get; }
        string CustomerManagementSystemDB_Docker { get; }
    }
}