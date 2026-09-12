using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Models;

namespace CustomerManagementSystem.BusinessLogic.CustomerFunctions;

public class CustomerActivation(IDbUtils dbUtils, ICustomerAuditLogger auditLogger)
{
    public async Task<ResponseModel<object>> DeactivateCustomer(string guid, string merchantId)
    {
        var response = await dbUtils.DeactivateCustomer(guid);

        if (response.Status == 200)
            await auditLogger.Log(guid, merchantId, "Deactivated");

        return response;
    }

    public async Task<ResponseModel<object>> ReactivateCustomer(string guid, string merchantId)
    {
        var response = await dbUtils.ReactivateCustomer(guid);

        if (response.Status == 200)
            await auditLogger.Log(guid, merchantId, "Reactivated");

        return response;
    }
}