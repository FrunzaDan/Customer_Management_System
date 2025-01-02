using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Models;

namespace CustomerManagementSystem.BusinessLogic.CustomerFunctions;

public class CustomerDeletion
{
    private readonly IDbUtils _dbUtils;

    public CustomerDeletion(IDbUtils dbUtils)
    {
        _dbUtils = dbUtils;
    }

    public async Task<ResponseModel<object>> DeleteCustomer(string guid)
    {
        return await _dbUtils.DeleteCustomer(guid);
    }
}