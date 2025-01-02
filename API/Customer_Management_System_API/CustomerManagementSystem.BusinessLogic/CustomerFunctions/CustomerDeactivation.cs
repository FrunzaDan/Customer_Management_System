using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Models;

namespace CustomerManagementSystem.BusinessLogic.CustomerFunctions;

public class CustomerDeactivation(IDbUtils dbUtils)
{
    public async Task<ResponseModel<object>> DeactivateCustomer(string guid)
    {
        return await dbUtils.DeactivateCustomer(guid);
    }
}