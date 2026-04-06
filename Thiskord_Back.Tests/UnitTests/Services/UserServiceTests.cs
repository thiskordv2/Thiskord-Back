using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Thiskord_Back.Services;
using Thiskord_Back.Models.Account;
using Thiskord_Back.Tests.Helpers;
using Moq;

namespace Thiskord_Back.Tests.UnitTests.Services
{
    public class UserServiceTests : IClassFixture<TestDatabaseFixture>
    {
        private readonly UserService _userService;
        
        public UserServiceTests(TestDatabaseFixture fixture)
        {
            var mockDbService = new Mock<IDbConnectionService>();
            mockDbService
                .Setup(db => db.CreateConnection())
                .Returns(() => new SqlConnection(fixture.ConnectionString));

            _userService = new UserService(mockDbService.Object, null);
        }

        [Fact]
        public void UserService_GetAllUsers_ReturnListOfUsers()
        {
            var res = _userService.GetAllUsers();
            res.Should().BeOfType<List<UserAccount>>();
            res.Count.Should().BeGreaterThan(0);
        }
    }    
}

