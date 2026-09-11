using CustomerManagementSystem.BusinessLogic.AuthFunctions;
using CustomerManagementSystem.DataAccess.DBConnection;
using CustomerManagementSystem.Domain.Configuration;
using CustomerManagementSystem.Domain.Models;
using Moq;

namespace CustomerManagementSystem.Tests.AuthFunctions;

public class JwtCreationTests
{
    private static Mock<IAppSettingsConfig> CreateConfig(string accessTokenTimeout = "15")
    {
        var config = new Mock<IAppSettingsConfig>();
        config.Setup(c => c.SecureJwtKey)
            .Returns("UGxlYXNlIHN0b3JlIHRoaXMgc2VjdXJpdHkga2V5IGluIGEgc2VjdXJlIGVudmlyb25tZW50IQ==");
        config.Setup(c => c.JwtIssuer).Returns("https://localhost:7145/");
        config.Setup(c => c.JwtAudience).Returns("https://localhost:7145/");
        config.Setup(c => c.AccessTokenTimeout).Returns(accessTokenTimeout);
        return config;
    }

    private static MerchantCredentials Credentials => new()
    {
        MerchantId = "TestMerchantID",
        MerchantPassword = "Merchant123",
    };

    [Fact]
    public async Task GenerateBearerJwt_ReturnsAToken_WhenCredentialsAreValid()
    {
        var dbUtils = new Mock<IDbUtils>();
        dbUtils.Setup(d => d.CheckMerchantCredentialsFromDb(It.IsAny<MerchantCredentials>()))
            .ReturnsAsync(new ResponseModel<int?>(200, "Success!", 1801));
        var jwtCreation = new JwtCreation(CreateConfig().Object, dbUtils.Object);

        var result = await jwtCreation.GenerateBearerJwt(Credentials);

        Assert.Equal(200, result.Status);
        var data = Assert.IsType<AccessTokenResponse>(result.Data);
        Assert.False(string.IsNullOrWhiteSpace(data.AccessToken));
        Assert.True(DateTime.Parse(data.ValidUntil!) > DateTime.UtcNow);
    }

    [Fact]
    public async Task GenerateBearerJwt_ReturnsForbidden_WhenCredentialsAreRejectedByTheDb()
    {
        var dbUtils = new Mock<IDbUtils>();
        dbUtils.Setup(d => d.CheckMerchantCredentialsFromDb(It.IsAny<MerchantCredentials>()))
            .ReturnsAsync(new ResponseModel<int?>(403, "Invalid merchant credentials."));
        var jwtCreation = new JwtCreation(CreateConfig().Object, dbUtils.Object);

        var result = await jwtCreation.GenerateBearerJwt(Credentials);

        Assert.Equal(403, result.Status);
        Assert.Equal("Invalid merchant credentials.", result.ResponseMessage);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GenerateBearerJwt_ReturnsForbidden_WithoutTouchingTheDb_WhenMerchantIdIsMissing(string? merchantId)
    {
        var dbUtils = new Mock<IDbUtils>();
        var jwtCreation = new JwtCreation(CreateConfig().Object, dbUtils.Object);
        var credentials = new MerchantCredentials { MerchantId = merchantId, MerchantPassword = "Merchant123" };

        var result = await jwtCreation.GenerateBearerJwt(credentials);

        Assert.Equal(403, result.Status);
        dbUtils.Verify(d => d.CheckMerchantCredentialsFromDb(It.IsAny<MerchantCredentials>()), Times.Never);
    }

    [Fact]
    public async Task GenerateBearerJwt_ReturnsServerError_WhenAccessTokenTimeoutIsNotConfiguredAsANumber()
    {
        var dbUtils = new Mock<IDbUtils>();
        dbUtils.Setup(d => d.CheckMerchantCredentialsFromDb(It.IsAny<MerchantCredentials>()))
            .ReturnsAsync(new ResponseModel<int?>(200, "Success!", 1801));
        var jwtCreation = new JwtCreation(CreateConfig(accessTokenTimeout: "not-a-number").Object, dbUtils.Object);

        var result = await jwtCreation.GenerateBearerJwt(Credentials);

        // BuildTokenDescriptor() calls double.Parse(AccessTokenTimeout) directly (not TryParse) while
        // building the token, which runs before GenerateBearerJwt's own double.TryParse check further
        // down — so an unparseable value throws here and is caught by the method's generic catch block,
        // never reaching the dedicated "Invalid AccessTokenTimeout configuration." message. Both paths
        // return 500, so behavior is still correct, but only the generic message is actually reachable.
        Assert.Equal(500, result.Status);
        Assert.StartsWith("An error occurred", result.ResponseMessage);
    }
}
