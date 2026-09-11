using CustomerManagementSystem.BusinessLogic.CustomerFunctions;
using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Models;
using Moq;

namespace CustomerManagementSystem.Tests.CustomerFunctions;

public class CustomerRegistrationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-an-email")]
    public async Task RegisterCustomerFunction_RejectsInvalidEmail_WithoutTouchingTheDb(string? email)
    {
        var dbUtils = new Mock<IDbUtils>();
        var registration = new CustomerRegistration(dbUtils.Object);
        var request = new CustomerModel { Email = email, Msisdn = "123456789" };

        var result = await registration.RegisterCustomerFunction(request);

        Assert.Equal(400, result.Status);
        Assert.Contains("Email", result.ResponseMessage);
        dbUtils.Verify(d => d.RegisterCustomer(It.IsAny<CustomerModel>()), Times.Never);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("123")]
    public async Task RegisterCustomerFunction_RejectsInvalidMsisdn_WithoutTouchingTheDb(string? msisdn)
    {
        var dbUtils = new Mock<IDbUtils>();
        var registration = new CustomerRegistration(dbUtils.Object);
        var request = new CustomerModel { Email = "dan@example.com", Msisdn = msisdn };

        var result = await registration.RegisterCustomerFunction(request);

        Assert.Equal(400, result.Status);
        Assert.Contains("MSISDN", result.ResponseMessage);
        dbUtils.Verify(d => d.RegisterCustomer(It.IsAny<CustomerModel>()), Times.Never);
    }

    [Fact]
    public async Task RegisterCustomerFunction_AlwaysGeneratesAFreshServerSideGuid_IgnoringAnyClientSuppliedValue()
    {
        var dbUtils = new Mock<IDbUtils>();
        CustomerModel? capturedRequest = null;
        dbUtils.Setup(d => d.RegisterCustomer(It.IsAny<CustomerModel>()))
            .Callback<CustomerModel>(c => capturedRequest = c)
            .ReturnsAsync(new ResponseModel<object>(0, "Customer created successfully."));
        var registration = new CustomerRegistration(dbUtils.Object);
        const string clientSuppliedGuid = "11111111-1111-1111-1111-111111111111";
        var request = new CustomerModel
        {
            Guid = clientSuppliedGuid,
            Email = "dan@example.com",
            Msisdn = "123456789",
        };

        var result = await registration.RegisterCustomerFunction(request);

        Assert.Equal(0, result.Status);
        dbUtils.Verify(d => d.RegisterCustomer(It.IsAny<CustomerModel>()), Times.Once);
        Assert.NotNull(capturedRequest!.Guid);
        Assert.NotEqual(clientSuppliedGuid, capturedRequest.Guid);
        Assert.True(Guid.TryParse(capturedRequest.Guid, out _));
    }
}
