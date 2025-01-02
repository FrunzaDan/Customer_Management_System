using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Models;

namespace CustomerManagementSystem.BusinessLogic.CustomerFunctions;

public class CustomerDeletion(IDbUtils dbUtils)
{
    public async Task<ResponseModel<object>> DeleteCustomer(string guid)
    {
        return await dbUtils.DeleteCustomer(guid);
    }
}