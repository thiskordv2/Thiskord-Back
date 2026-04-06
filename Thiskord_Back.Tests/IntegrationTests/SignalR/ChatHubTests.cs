using FluentAssertions;
using Microsoft.AspNetCore.SignalR.Client;
using System.Diagnostics;
using Thiskord_Back.Tests.Helpers;
using Xunit;

namespace Thiskord_Back.Tests.IntegrationTests.SignalR
{
    public class ChatHubTests : IClassFixture<TestWebAppFactory>, IClassFixture<TestDatabaseFixture>
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

            var connection = new HubConnectionBuilder()
                .WithUrl(hubUrl, options =>
                {
                    options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling;
                    options.AccessTokenProvider = () => Task.FromResult<string?>(token);
                    options.HttpMessageHandlerFactory = _ => _factory.Server.CreateHandler();
                })
                .Build();
            
            connection.Closed += ex =>
            {
                if (ex != null)
                    throw new Exception($"Connexion fermée avec erreur : {ex.Message}", ex);
                return Task.CompletedTask;
            };
            return connection;
        }

        [Fact]
        public async Task ChatHub_JoinChannel_ReceivesMessageHistory()
        {
            var connection = CreateConnection();
            var tcs = new TaskCompletionSource<List<object>>(TaskCreationOptions.RunContinuationsAsynchronously);
            
            connection.On<List<object>>("LoadMessages", msgs => tcs.SetResult(msgs));

            await connection.StartAsync();
            await connection.InvokeAsync("JoinChannel", 1);

            var result = await Task.WhenAny(tcs.Task, Task.Delay(2000));
            result.Should().Be(tcs.Task, "Le serveur aurait dû renvoyer l'historique.");

            var messages = await tcs.Task;
            messages.Should().NotBeNull();

            await connection.StopAsync();
        }

        [Fact]
        public async Task ChatHub_SendMessage_BroadcastsToGroup()
        {
            var sender = CreateConnection(1, "EMRE");
            var receiver = CreateConnection(2, "ROBIN");
            var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);

            receiver.On<int, string, string, string>("ReceiveMessage", 
                (id, user, text, dateTime) => tcs.TrySetResult(text));

            await sender.StartAsync();
            await receiver.StartAsync();
            await sender.InvokeAsync("JoinChannel", 1);
            await receiver.InvokeAsync("JoinChannel", 1);

            await sender.InvokeAsync("SendMessage", 1, "Hello from test!");

            var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(2000));
            completedTask.Should().Be(tcs.Task);
            
            var sended =  await tcs.Task;
            sended.Should().Be("Hello from test!");

            await sender.StopAsync();
            await receiver.StopAsync();
        }

        [Fact]
        public async Task ChatHub_Latency_IsBelow200ms()
        {
            var sender = CreateConnection(1, "SENDER");
            var receiver = CreateConnection(2, "RECEIVER");
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var sw = new Stopwatch();

            await sender.StartAsync();
            await receiver.StartAsync();
            await sender.InvokeAsync("JoinChannel", 1);
            await receiver.InvokeAsync("JoinChannel", 1);

            await sender.InvokeAsync("SendMessage", 1, "warmup");

            receiver.On<int, string, string, string>("ReceiveMessage", (id, user, text, dt) =>
            {
                if (text == "latency-test") {
                    sw.Stop();
                    tcs.TrySetResult(true);
                }
            });

            sw.Start();
            await sender.InvokeAsync("SendMessage", 1, "latency-test");

            var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(1000));
            completedTask.Should().Be(tcs.Task, "Le message n'a pas été reçu à temps.");
            
            sw.ElapsedMilliseconds.Should().BeLessThan(200, 
                $"La latence était de {sw.ElapsedMilliseconds}ms, ce qui dépasse l'objectif de 200ms.");

            await sender.StopAsync();
            await receiver.StopAsync();
        }

        [Fact]
        public async Task ChatHub_LeaveChannel_StopsReceivingMessages()
        {
            var sender = CreateConnection(1, "EMRE");
            var receiver = CreateConnection(2, "ROBIN");
            bool received = false;

            receiver.On<int, string, string, string>("ReceiveMessage", (id, u, t, d) => received = true);

            await sender.StartAsync();
            await receiver.StartAsync();
            
            await sender.InvokeAsync("JoinChannel", 1);
            await receiver.InvokeAsync("JoinChannel", 1);
            await receiver.InvokeAsync("LeaveChannel", 1); 

            await sender.InvokeAsync("SendMessage", 1, "Invisible message");
            
            await Task.Delay(300); 

            received.Should().BeFalse();

            await sender.StopAsync();
            await receiver.StopAsync();
        }
    }
}
