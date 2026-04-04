using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Moq;
using Thiskord_Back.Models.Auth;
using Thiskord_Back.Services;
using Thiskord_Back.Tests.Setup;

namespace Thiskord_Back.Tests.UnitTests.Services
{
    public class AuthServiceTests : IClassFixture<TestDatabaseFixture>
    {
        private readonly AuthService _authService;
        private readonly TestDatabaseFixture _fixture;
        
        public AuthServiceTests(TestDatabaseFixture fixture)
        {
            _fixture = fixture;
            var mockDbService = new Mock<IDbConnectionService>();
            mockDbService
                .Setup(db => db.CreateConnection())
                .Returns(() => new SqlConnection(fixture.ConnectionString));
            
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["Jwt:Key"]).Returns("une-cle-secrete-suffisamment-longue-32chars");
            mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("test-issuer");
            mockConfig.Setup(c => c["Jwt:Audience"]).Returns("test-audience");
            
            _authService = new AuthService(mockDbService.Object, mockConfig.Object);
        }

        [Fact]
        public void AccountService_AuthLogin_ReturnAuthenticatedUser()
        {
            string username = "EMRE";
            string password = "EMRE";
            AuthenticatedUser res = _authService.AuthLogin(username, password);
            
            res.Should().NotBeNull();
            res.Should().BeOfType<AuthenticatedUser>();
            res.user.user_name.Should().Be(username);
            res.token.Should().NotBeNullOrEmpty();
        }
    }
}

