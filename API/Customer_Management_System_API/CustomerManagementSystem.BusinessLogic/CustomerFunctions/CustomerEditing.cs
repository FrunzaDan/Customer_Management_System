using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Models;

namespace CustomerManagementSystem.BusinessLogic.CustomerFunctions;

public class CustomerEditing(IDbUtils dbUtils)
{
    public async Task<ResponseModel<object>> EditCustomerFunction(CustomerModel request)
    {
        return await dbUtils.EditCustomer(request);
    }
}