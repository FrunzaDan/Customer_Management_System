using CustomerManagementSystem.BusinessLogic.CustomerFunctions;
using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Models;
using Moq;

namespace CustomerManagementSystem.Tests.CustomerFunctions;

public class CustomerActivationTests
{
    private const string Guid = "3fa85f64-5717-4562-b3fc-2c963f66afa6";

    [Fact]
    public async Task DeactivateCustomer_DelegatesToTheDbLayerWithTheGivenGuid()
    {
        var dbUtils = new Mock<IDbUtils>();
        var expected = new ResponseModel<object>(0, "Customer deactivated successfully.");
        dbUtils.Setup(d => d.DeactivateCustomer(Guid)).ReturnsAsync(expected);
        var activation = new CustomerActivation(dbUtils.Object);

        var result = await activation.DeactivateCustomer(Guid);

        Assert.Same(expected, result);
        dbUtils.Verify(d => d.DeactivateCustomer(Guid), Times.Once);
        dbUtils.Verify(d => d.ReactivateCustomer(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ReactivateCustomer_DelegatesToTheDbLayerWithTheGivenGuid()
    {
        var dbUtils = new Mock<IDbUtils>();
        var expected = new ResponseModel<object>(0, "Customer reactivated successfully.");
        dbUtils.Setup(d => d.ReactivateCustomer(Guid)).ReturnsAsync(expected);
        var activation = new CustomerActivation(dbUtils.Object);

        var result = await activation.ReactivateCustomer(Guid);

        Assert.Same(expected, result);
        dbUtils.Verify(d => d.ReactivateCustomer(Guid), Times.Once);
        dbUtils.Verify(d => d.DeactivateCustomer(It.IsAny<string>()), Times.Never);
    }
}
