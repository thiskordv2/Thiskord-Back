using System.Diagnostics;
using Microsoft.Data.SqlClient;
using Thiskord_Back.Services;
using Thiskord_Back.Tests.Helpers;
using FluentAssertions;
using Moq;

namespace Thiskord_Back.Tests.IntegrationTests.Database
{
    public class MessageServiceTests : IClassFixture<TestDatabaseFixture>
    {
        private readonly TestDatabaseFixture _dbFixture;
        private readonly MessageService _messageService;
        private readonly IDbConnectionService _dbConnectionService;

        public MessageServiceTests(TestDatabaseFixture dbFixture)
        {
            _dbFixture = dbFixture;

            var connectionServiceMock = new Mock<IDbConnectionService>();
            connectionServiceMock.Setup(s => s.CreateConnection())
                .Returns(() => new SqlConnection(_dbFixture.ConnectionString));
            
            _dbConnectionService = connectionServiceMock.Object;
            
            var logServiceMock = new Mock<ILogService>(); 

            _messageService = new MessageService(_dbConnectionService, logServiceMock.Object);
        }

        [Fact]
        public async Task GetAllMessage_Performance_ShouldBeUnder100ms()
        {
            int channelId = 1;
            var sw = new Stopwatch();

            using (var conn = _dbConnectionService.CreateConnection()) { await conn.OpenAsync(); }

            sw.Start();
            var messages = await _messageService.GetAllMessage(channelId);
            sw.Stop();

            sw.ElapsedMilliseconds.Should().BeLessThan(100, 
                $"La récupération des messages a pris {sw.ElapsedMilliseconds}ms (Limite: 100ms)");
            
            messages.Should().NotBeNull();
        }

        [Fact]
        public async Task SendMessage_Performance_ShouldBeUnder100ms()
        {
            int channelId = 1;
            int userId = 1;
            string content = "Message de test performance";
            var sw = new Stopwatch();

            sw.Start();
            var result = await _messageService.SendMessage(channelId, userId, content);
            sw.Stop();

            sw.ElapsedMilliseconds.Should().BeLessThan(100, 
                $"L'insertion du message a pris {sw.ElapsedMilliseconds}ms (Limite: 100ms)");
            
            result.Should().NotBeNull();
            result.Content.Should().Be(content);
        }

        [Fact]
        public async Task DeleteMessage_ShouldRemoveFromDatabase()
        {
            var msg = await _messageService.SendMessage(1, 1, "A supprimer");

            await _messageService.DeleteMessage(msg.Id, 1);

            var allMessages = await _messageService.GetAllMessage(1);
            allMessages.Should().NotContain(m => m.Id == msg.Id);
        }
    }
}
