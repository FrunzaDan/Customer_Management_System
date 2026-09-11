using CustomerManagementSystem.BusinessLogic.CustomerFunctions;
using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Models;
using Moq;

namespace CustomerManagementSystem.Tests.CustomerFunctions;

public class CustomerDeletionTests
{
    [Fact]
    public async Task DeleteCustomer_DelegatesToTheDbLayerWithTheGivenGuid()
    {
        const string guid = "3fa85f64-5717-4562-b3fc-2c963f66afa6";
        var dbUtils = new Mock<IDbUtils>();
        var expected = new ResponseModel<object>(0, "Customer deleted successfully.");
        dbUtils.Setup(d => d.DeleteCustomer(guid)).ReturnsAsync(expected);
        var deletion = new CustomerDeletion(dbUtils.Object);

        var result = await deletion.DeleteCustomer(guid);

        Assert.Same(expected, result);
        dbUtils.Verify(d => d.DeleteCustomer(guid), Times.Once);
    }

    [Fact]
    public async Task DeleteCustomer_PropagatesABusinessRuleRejection_WithoutModifyingIt()
    {
        // Mirrors the real usp_deleteCustomer rule: an active customer can't be deleted directly.
        const string guid = "3fa85f64-5717-4562-b3fc-2c963f66afa6";
        var dbUtils = new Mock<IDbUtils>();
        var expected = new ResponseModel<object>(409, "Customer must be deactivated before it can be deleted.");
        dbUtils.Setup(d => d.DeleteCustomer(guid)).ReturnsAsync(expected);
        var deletion = new CustomerDeletion(dbUtils.Object);

        var result = await deletion.DeleteCustomer(guid);

        Assert.Equal(409, result.Status);
        Assert.Equal(expected.ResponseMessage, result.ResponseMessage);
    }
}
