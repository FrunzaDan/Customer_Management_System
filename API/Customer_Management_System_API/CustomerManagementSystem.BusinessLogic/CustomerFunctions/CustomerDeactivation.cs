using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Models;

namespace CustomerManagementSystem.BusinessLogic.CustomerFunctions;

public class CustomerDeactivation
{
    private readonly IDbUtils _dbUtils;

    public CustomerDeactivation(IDbUtils dbUtils)
    {
        _dbUtils = dbUtils;
    }

    public async Task<ResponseModel<object>> DeactivateCustomer(string customerGuid)
    {
        return await _dbUtils.DeactivateCustomer(customerGuid);
    }
}