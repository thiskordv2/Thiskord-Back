using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Moq;
using Thiskord_Back.Models.Account;
using Thiskord_Back.Services;
using Thiskord_Back.Tests.Setup;

namespace Thiskord_Back.Tests.UnitTests.Services
{
    public class InscriptionServiceTests : IClassFixture<TestDatabaseFixture>
    {
        private readonly InscriptionService _inscriptionService;

        public InscriptionServiceTests(TestDatabaseFixture fixture)
        {
            var mockDbService = new Mock<IDbConnectionService>();
            mockDbService
                .Setup(db => db.CreateConnection())
                .Returns(() => new SqlConnection(fixture.ConnectionString));
            
            _inscriptionService = new InscriptionService(mockDbService.Object, null);
        }

        [Fact]
        public async Task InscriptionService_InscriptionUser_ReturnUserAccount()
        {
            var user = await _inscriptionService.InscriptionUser("TestUser", "atest@mail.com", "motdepasseOK", "");
            user.Should().NotBeNull();
            user.Should().BeOfType<UserAccount>();
            user.user_id.Should().BeGreaterThan(0);
            user.user_name.Should().Be("TestUser");
            user.user_mail.Should().Be("atest@mail.com");
        }
    }
}