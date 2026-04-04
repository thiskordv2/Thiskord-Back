// UnitTests/Hubs/ChatHubTests.cs
using FluentAssertions;
using Microsoft.AspNetCore.SignalR.Client;
using Thiskord_Back.Tests.Helpers;

namespace Thiskord_Back.Tests.IntegrationTests.SignalR
{
    public class ChatHubTests : IClassFixture<TestWebAppFactory>, 
                                IClassFixture<TestDatabaseFixture>
    {
        private readonly TestWebAppFactory _factory;
        private readonly TestDatabaseFixture _dbFixture;

        public ChatHubTests(TestWebAppFactory factory, TestDatabaseFixture dbFixture)
        {
            _factory = factory;
            _factory.ConnectionString = dbFixture.ConnectionString;
            _dbFixture = dbFixture;
        }

        private HubConnection CreateConnection(int userId = 1, string username = "EMRE")
        {
            var token = TestJwtHelper.GenerateToken(userId, username);
            var hubUrl = new Uri(_factory.Server.BaseAddress, "chatHub").ToString();
            
            return new HubConnectionBuilder()
                .WithUrl(hubUrl, options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult<string?>(token);
                    options.HttpMessageHandlerFactory = _ => _factory.Server.CreateHandler();
                })
                .Build();
        }

        [Fact]
        public async Task ChatHub_JoinChannel_ReceivesMessageHistory()
        {
            var connection = CreateConnection();
            var messages = new List<object>();
            connection.On<List<object>>("LoadMessages", msgs => messages = msgs);

            await connection.StartAsync();
            await connection.InvokeAsync("JoinChannel", 1);
            await Task.Delay(500);
            
            messages.Should().NotBeNull();
            await connection.StopAsync();
        }

        [Fact]
        public async Task ChatHub_SendMessage_BroadcastsToGroup()
        {
            var sender = CreateConnection(1, "EMRE");
            var receiver = CreateConnection(2, "ROBIN");

            string? receivedText = null;
            receiver.On<int, string, string, string>("ReceiveMessage",
                (id, user, text, dateTime) => receivedText = text);

            await sender.StartAsync();
            await receiver.StartAsync();
            await sender.InvokeAsync("JoinChannel", 1);
            await receiver.InvokeAsync("JoinChannel", 1);
            await sender.InvokeAsync("SendMessage", 1, "Hello from test!");
            await Task.Delay(500);

            receivedText.Should().Be("Hello from test!");

            await sender.StopAsync();
            await receiver.StopAsync();
        }

        [Fact]
        public async Task ChatHub_LeaveChannel_StopsReceivingMessages()
        {
            var sender = CreateConnection(1, "EMRE");
            var receiver = CreateConnection(2, "ROBIN");

            string? receivedText = null;
            receiver.On<int, string, string, string>("ReceiveMessage",
                (id, user, text, dateTime) => receivedText = text);

            await sender.StartAsync();
            await receiver.StartAsync();
            await sender.InvokeAsync("JoinChannel", 1);
            await receiver.InvokeAsync("JoinChannel", 1);
            await receiver.InvokeAsync("LeaveChannel", 1); // quitte le channel
            await sender.InvokeAsync("SendMessage", 1, "Tu devrais pas recevoir ça");
            await Task.Delay(500);

            receivedText.Should().BeNull(); // le receiver n'a rien reçu

            await sender.StopAsync();
            await receiver.StopAsync();
        }
    }
}