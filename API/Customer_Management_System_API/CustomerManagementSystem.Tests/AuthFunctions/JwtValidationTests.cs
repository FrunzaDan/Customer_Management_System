using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CustomerManagementSystem.BusinessLogic.AuthFunctions;
using CustomerManagementSystem.Domain.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace CustomerManagementSystem.Tests.AuthFunctions;

public class JwtValidationTests
{
    private const string Key = "UGxlYXNlIHN0b3JlIHRoaXMgc2VjdXJpdHkga2V5IGluIGEgc2VjdXJlIGVudmlyb25tZW50IQ==";
    private const string Issuer = "https://localhost:7145/";
    private const string Audience = "https://localhost:7145/";

    private static JwtValidation CreateValidator()
    {
        var config = new Mock<IAppSettingsConfig>();
        config.Setup(c => c.SecureJwtKey).Returns(Key);
        config.Setup(c => c.JwtIssuer).Returns(Issuer);
        config.Setup(c => c.JwtAudience).Returns(Audience);
        return new JwtValidation(config.Object);
    }

    private static string MintToken(
        string merchantId = "TestMerchantID",
        string signingKey = Key,
        string issuer = Issuer,
        string audience = Audience,
        DateTime? expires = null,
        bool includeSidClaim = true)
    {
        var claims = includeSidClaim
            ? new List<Claim> { new(ClaimTypes.Sid, merchantId) }
            : new List<Claim>();

        var tokenExpires = expires ?? DateTime.UtcNow.AddMinutes(15);
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            // NotBefore defaults to UtcNow, which would be *after* an already-expired Expires and make
            // JwtSecurityTokenHandler.CreateToken itself throw — so for an expired-token test, anchor
            // NotBefore safely before Expires instead of leaving it at the default.
            NotBefore = tokenExpires < DateTime.UtcNow ? tokenExpires.AddMinutes(-10) : DateTime.UtcNow,
            Expires = tokenExpires,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(signingKey)), SecurityAlgorithms.HmacSha256Signature),
            Issuer = issuer,
            Audience = audience,
        };

        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(handler.CreateToken(descriptor));
    }

    [Fact]
    public void ValidateToken_ReturnsSuccess_ForAWellFormedToken()
    {
        var validator = CreateValidator();
        var token = MintToken();

        var result = validator.ValidateToken(token);

        Assert.Equal(200, result.Status);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ValidateToken_ReturnsUnauthorized_ForAnEmptyToken(string? token)
    {
        var validator = CreateValidator();

        var result = validator.ValidateToken(token);

        Assert.Equal(500, result.Status);
        Assert.Contains("Empty JWT", result.ResponseMessage);
    }

    [Fact]
    public void ValidateToken_ReturnsUnauthorized_ForAGarbageString()
    {
        var validator = CreateValidator();

        var result = validator.ValidateToken("this-is-not-a-jwt");

        Assert.Equal(500, result.Status);
        Assert.Contains("valid format", result.ResponseMessage);
    }

    [Fact]
    public void ValidateToken_ReturnsUnauthorized_ForAnExpiredToken()
    {
        var validator = CreateValidator();
        var token = MintToken(expires: DateTime.UtcNow.AddMinutes(-1));

        var result = validator.ValidateToken(token);

        Assert.Equal(500, result.Status);
    }

    [Fact]
    public void ValidateToken_ReturnsUnauthorized_ForATokenSignedWithAnUnknownKey()
    {
        var validator = CreateValidator();
        var token = MintToken(signingKey: "VGhpcyBpcyBhIGRpZmZlcmVudCBzaWduaW5nIGtleSE=");

        var result = validator.ValidateToken(token);

        Assert.Equal(500, result.Status);
    }

    [Fact]
    public void ValidateToken_ReturnsUnauthorized_ForAWrongIssuer()
    {
        var validator = CreateValidator();
        var token = MintToken(issuer: "https://not-us.example/");

        var result = validator.ValidateToken(token);

        Assert.Equal(500, result.Status);
    }

    [Fact]
    public void ValidateToken_ReturnsUnauthorized_ForAWrongAudience()
    {
        var validator = CreateValidator();
        var token = MintToken(audience: "https://not-us.example/");

        var result = validator.ValidateToken(token);

        Assert.Equal(500, result.Status);
    }

    [Fact]
    public void ValidateToken_ReturnsUnauthorized_WhenTheSidClaimIsMissing()
    {
        var validator = CreateValidator();
        var token = MintToken(includeSidClaim: false);

        var result = validator.ValidateToken(token);

        Assert.Equal(500, result.Status);
        Assert.Contains("Invalid claims", result.ResponseMessage);
    }
}
