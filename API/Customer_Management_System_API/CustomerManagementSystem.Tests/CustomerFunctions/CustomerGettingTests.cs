using CustomerManagementSystem.BusinessLogic.CustomerFunctions;
using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Models;
using Moq;

namespace CustomerManagementSystem.Tests.CustomerFunctions;

public class CustomerGettingTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetCustomerFunction_RejectsAnEmptySearchVariable_WithoutTouchingTheDb(string? searchVariable)
    {
        var dbUtils = new Mock<IDbUtils>();
        var getting = new CustomerGetting(dbUtils.Object);
        var request = new GetCustomerRequest { SearchVariable = searchVariable };

        var result = await getting.GetCustomerFunction(request);

        Assert.Equal(404, result.Status);
        dbUtils.Verify(d => d.GetCustomer(It.IsAny<GetCustomerRequest>()), Times.Never);
    }

    [Fact]
    public async Task GetCustomerFunction_RejectsASearchVariableThatIsNeitherGuidMsisdnNorEmail()
    {
        var dbUtils = new Mock<IDbUtils>();
        var getting = new CustomerGetting(dbUtils.Object);
        var request = new GetCustomerRequest { SearchVariable = "not-a-valid-search-term" };

        var result = await getting.GetCustomerFunction(request);

        Assert.Equal(404, result.Status);
        dbUtils.Verify(d => d.GetCustomer(It.IsAny<GetCustomerRequest>()), Times.Never);
    }

    [Theory]
    [InlineData("3fa85f64-5717-4562-b3fc-2c963f66afa6", 1)] // GUID
    [InlineData("123456789", 2)] // MSISDN
    [InlineData("dan@example.com", 3)] // Email
    public async Task GetCustomerFunction_DetectsTheSearchOptionFromTheSearchVariableShape(string searchVariable, int expectedSearchOption)
    {
        var dbUtils = new Mock<IDbUtils>();
        GetCustomerRequest? capturedRequest = null;
        dbUtils.Setup(d => d.GetCustomer(It.IsAny<GetCustomerRequest>()))
            .Callback<GetCustomerRequest>(r => capturedRequest = r)
            .ReturnsAsync(new ResponseModel<object>(200, "Success!"));
        var getting = new CustomerGetting(dbUtils.Object);
        var request = new GetCustomerRequest { SearchVariable = searchVariable };

        await getting.GetCustomerFunction(request);

        Assert.Equal(expectedSearchOption, capturedRequest!.SearchOption);
    }

    [Fact]
    public async Task GetCustomersFunction_ReturnsWhateverTheDbLayerReturns()
    {
        var dbUtils = new Mock<IDbUtils>();
        var expected = new ResponseModel<object>(200, "Success!", new List<CustomerModel>());
        dbUtils.Setup(d => d.GetCustomers()).ReturnsAsync(expected);
        var getting = new CustomerGetting(dbUtils.Object);

        var result = await getting.GetCustomersFunction();

        Assert.Same(expected, result);
        dbUtils.Verify(d => d.GetCustomers(), Times.Once);
    }
}
