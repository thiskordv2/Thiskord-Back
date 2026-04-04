using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Moq;
using Thiskord_Back.Services;
using Thiskord_Back.Tests.Setup;
using Thiskord_Back.Models.Channel;

namespace Thiskord_Back.Tests.UnitTests.Services
{
    public class ChannelServiceTests : IClassFixture<TestDatabaseFixture>
    {
        private readonly ChannelService _channelService;
        private readonly TestDatabaseFixture _fixture;

        public ChannelServiceTests(TestDatabaseFixture fixture)
        {
            _fixture = fixture;
            var mockDbService = new Mock<IDbConnectionService>();
            mockDbService
                .Setup(db => db.CreateConnection())
                .Returns(() => new SqlConnection(fixture.ConnectionString));
            
            _channelService = new ChannelService(mockDbService.Object, null);
        }

        [Fact]
        public void ChannelService_Create_ReturnsChannel()
        {
            string newChannelName = "NewChannel";
            string newChannelDescription = "NewChannelDescription";
            var res = _channelService.Create(newChannelName, newChannelDescription);
            res.Should().BeOfType<Channel>();
            res.name.Should().Be(newChannelName);
            res.description.Should().Be(newChannelDescription);
        }

        [Fact]
        public void ChannelService_DeleteById_ReturnsVoid()
        {
            int channelId = 1;
            _channelService.DeleteById(channelId);
            using var conn = new SqlConnection(_fixture.ConnectionString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT * FROM Channel WHERE channel_id = @id", conn);
            cmd.Parameters.AddWithValue("@id", channelId);
            string? result = cmd.ExecuteScalar()?.ToString();
            result.Should().BeNull();
        }

        [Fact]
        public void ChannelService_Update_ReturnsChannel()
        {
            int channelId = 2;
            string newChannelName = "NewChannelName";
            string newChannelDescription = "NewChannelDescription";
            var res = _channelService.Create(newChannelName, newChannelDescription);
            res.Should().BeOfType<Channel>();
            res.name.Should().Be(newChannelName);
            res.description.Should().Be(newChannelDescription);
        }

        [Fact]
        public void ChannelService_GetChannelsByProjectId_ReturnsListChannels()
        {
            int projectId = 2;
            List<Channel> channels = _channelService.GetChannelsByProjectId(projectId);
            channels.Should().BeOfType<List<Channel>>();
            channels.Count.Should().Be(2);
        }
    }
}