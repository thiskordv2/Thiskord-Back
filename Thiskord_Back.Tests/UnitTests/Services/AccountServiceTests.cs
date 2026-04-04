using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Thiskord_Back.Models.Account;
using Thiskord_Back.Services;
using Thiskord_Back.Tests.Helpers;

namespace Thiskord_Back.Tests.UnitTests.Services
{
    public class AccountServiceTests : IClassFixture<TestDatabaseFixture>
    {
        private readonly AccountService _accountService;
        private readonly TestDatabaseFixture _fixture;
        
        public AccountServiceTests(TestDatabaseFixture fixture)
        {
            _fixture = fixture;
            var mockDbService = new Mock<IDbConnectionService>();
            mockDbService
                .Setup(db => db.CreateConnection())
                .Returns(() => new SqlConnection(fixture.ConnectionString));
            
            _accountService = new AccountService(mockDbService.Object, null);
        }
        
        [Fact]
        public void AccountService_GetAccount_ReturnAccount()
        {
            var res = _accountService.getAccount(1);
            res.Should().BeOfType<UserAccount>();
            res.user_id.Should().Be(1);
        }

        [Fact]
        public void AccountService_PatchAccount_ReturnInt()
        {
            UserAccount account1 = new UserAccount(1, "Emre", "Emre@gmail.com", "https://mon-image.com/image.png");
            var res = _accountService.patchAccount(account1);
            var newUser = _accountService.getAccount(1);
            res.Should().Be(1);
            newUser.user_id.Should().Be(1);
            newUser.user_name.Should().Be("Emre");
            newUser.user_mail.Should().Be("Emre@gmail.com");
            newUser.user_picture.Should().Be("https://mon-image.com/image.png");
        }

        [Fact]
        public void AccountService_PatchPassword_ReturnInt()
        {
            var password = "monMotDePasseSécurisé";
            var account = new UserAccount(1, "ROBIN", "rob@rob.rob", "pic") { user_password = password };
            var res = _accountService.patchAccountPassword(account);
            res.Should().Be(1);
            using var conn = new SqlConnection(_fixture.ConnectionString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT user_password FROM Account WHERE user_id = @id", conn);
            cmd.Parameters.AddWithValue("@id", 1);
            string? hash = cmd.ExecuteScalar()?.ToString();
            BCrypt.Net.BCrypt.Verify(password, hash).Should().BeTrue();
            
        }
    }
}

