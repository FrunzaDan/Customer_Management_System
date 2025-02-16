using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Models;

namespace CustomerManagementSystem.BusinessLogic.CustomerFunctions;

public class CustomerActivation(IDbUtils dbUtils)
{
    public async Task<ResponseModel<object>> DeactivateCustomer(string guid)
    {
        return await dbUtils.DeactivateCustomer(guid);
    }
    
    public async Task<ResponseModel<object>> ReactivateCustomer(string guid)
    {
        return await dbUtils.ReactivateCustomer(guid);
    }
}