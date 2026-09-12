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

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetCustomersFunction_RejectsAnInvalidPageNumber_WithoutTouchingTheDb(int pageNumber)
    {
        var dbUtils = new Mock<IDbUtils>();
        var getting = new CustomerGetting(dbUtils.Object);
        var request = new GetCustomersRequest { PageNumber = pageNumber, PageSize = 10 };

        var result = await getting.GetCustomersFunction(request);

        Assert.Equal(400, result.Status);
        dbUtils.Verify(d => d.GetCustomers(It.IsAny<GetCustomersRequest>()), Times.Never);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)]
    public async Task GetCustomersFunction_RejectsAnInvalidPageSize_WithoutTouchingTheDb(int pageSize)
    {
        var dbUtils = new Mock<IDbUtils>();
        var getting = new CustomerGetting(dbUtils.Object);
        var request = new GetCustomersRequest { PageNumber = 1, PageSize = pageSize };

        var result = await getting.GetCustomersFunction(request);

        Assert.Equal(400, result.Status);
        dbUtils.Verify(d => d.GetCustomers(It.IsAny<GetCustomersRequest>()), Times.Never);
    }

    [Fact]
    public async Task GetCustomersFunction_RejectsAnInvalidSortColumn_WithoutTouchingTheDb()
    {
        var dbUtils = new Mock<IDbUtils>();
        var getting = new CustomerGetting(dbUtils.Object);
        var request = new GetCustomersRequest { SortColumn = "not-a-real-column" };

        var result = await getting.GetCustomersFunction(request);

        Assert.Equal(400, result.Status);
        dbUtils.Verify(d => d.GetCustomers(It.IsAny<GetCustomersRequest>()), Times.Never);
    }

    [Fact]
    public async Task GetCustomersFunction_RejectsAnInvalidSortDirection_WithoutTouchingTheDb()
    {
        var dbUtils = new Mock<IDbUtils>();
        var getting = new CustomerGetting(dbUtils.Object);
        var request = new GetCustomersRequest { SortDirection = "sideways" };

        var result = await getting.GetCustomersFunction(request);

        Assert.Equal(400, result.Status);
        dbUtils.Verify(d => d.GetCustomers(It.IsAny<GetCustomersRequest>()), Times.Never);
    }

    [Theory]
    [InlineData("NAME", "DESC", "name", "desc")]
    [InlineData(" Email ", " Asc ", "email", "asc")]
    public async Task GetCustomersFunction_NormalizesSortColumnAndDirectionToLowercase(
        string sortColumn, string sortDirection, string expectedColumn, string expectedDirection)
    {
        var dbUtils = new Mock<IDbUtils>();
        GetCustomersRequest? captured = null;
        dbUtils.Setup(d => d.GetCustomers(It.IsAny<GetCustomersRequest>()))
            .Callback<GetCustomersRequest>(r => captured = r)
            .ReturnsAsync(new ResponseModel<object>(200, "Success!"));
        var getting = new CustomerGetting(dbUtils.Object);
        var request = new GetCustomersRequest { SortColumn = sortColumn, SortDirection = sortDirection };

        await getting.GetCustomersFunction(request);

        Assert.Equal(expectedColumn, captured!.SortColumn);
        Assert.Equal(expectedDirection, captured.SortDirection);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetCustomersFunction_TreatsABlankSearchTermAsNoSearch(string? searchTerm)
    {
        var dbUtils = new Mock<IDbUtils>();
        GetCustomersRequest? captured = null;
        dbUtils.Setup(d => d.GetCustomers(It.IsAny<GetCustomersRequest>()))
            .Callback<GetCustomersRequest>(r => captured = r)
            .ReturnsAsync(new ResponseModel<object>(200, "Success!"));
        var getting = new CustomerGetting(dbUtils.Object);
        var request = new GetCustomersRequest { SearchTerm = searchTerm };

        await getting.GetCustomersFunction(request);

        Assert.Null(captured!.SearchTerm);
    }

    [Fact]
    public async Task GetCustomersFunction_ReturnsWhateverTheDbLayerReturns()
    {
        var dbUtils = new Mock<IDbUtils>();
        var expected = new ResponseModel<object>(200, "Success!",
            new PagedResponse<CustomerModel>(new List<CustomerModel>(), 0, 1, 10));
        var request = new GetCustomersRequest { PageNumber = 1, PageSize = 10 };
        dbUtils.Setup(d => d.GetCustomers(It.IsAny<GetCustomersRequest>())).ReturnsAsync(expected);
        var getting = new CustomerGetting(dbUtils.Object);

        var result = await getting.GetCustomersFunction(request);

        Assert.Same(expected, result);
        dbUtils.Verify(d => d.GetCustomers(It.IsAny<GetCustomersRequest>()), Times.Once);
    }
}
