using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Models;

namespace CustomerManagementSystem.BusinessLogic.CustomerFunctions;

public class CustomerEditing
{
    private readonly IDbUtils _dbUtils;

    public CustomerEditing(IDbUtils dbUtils)
    {
        _dbUtils = dbUtils;
    }

    public async Task<ResponseModel<object>> EditCustomerFunction(CustomerModel request)
    {
        return await _dbUtils.EditCustomer(request);
    }
}