using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Thiskord_Back.Controllers;
using Thiskord_Back.Models.Channel;
using Thiskord_Back.Services;

namespace Thiskord_Back.Tests.UnitTests.Controllers
{
    public class ChannelControllerTests
    {
        private readonly ChannelController _channelController;
        private readonly Mock<IChannelService> _mockChannelService;

        public ChannelControllerTests()
        {
            _mockChannelService = new Mock<IChannelService>();
            _channelController = new ChannelController(_mockChannelService.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
            }, "mock"));

            _channelController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task ChannelController_CreateChannel_ReturnsOk()
        {
            var request = new ChannelRequest() { name = "Test Channel", description = "This is a test channel", projectId = 1};
            var responseChannel = new Channel() { id = 1, name = request.name, description = request.description }; 
            _mockChannelService.Setup(s => s.Create(request.name, request.description, request.projectId, 1)).ReturnsAsync(responseChannel);
            
            var actualResult = await _channelController.CreateChannel(request);

            actualResult.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task ChannelController_DeleteChannel_ReturnsOk()
        {
            int id = 1;
            _mockChannelService.Setup(s => s.DeleteById(id)).Returns(Task.CompletedTask);
            var actualResult = await _channelController.DeleteChannel(id);
            actualResult.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task ChannelController_UpdateChannel_ReturnsOk()
        {
            int id = 1;
            string name = "Test Channel";
            string description = "This is a test channel";
            var channel = new Channel() { id = 1, name = "Test Channel", description = "This is a test channel" };
            _mockChannelService.Setup(s => s.Update(id, name, description)).ReturnsAsync(channel);

            var actionResult = await _channelController.UpdateChannel(new ChannelRequest() { name = name, description = description }, id);
            actionResult.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task ChannelController_GetChannelsByProjectId_ReturnsOk()
        {
            int projectId = 1;
            List<Channel> channels = new List<Channel>
            {
                new Channel() { id = 1, name = "Channel 1", description = "Description 1" },
                new Channel() { id = 2, name = "Channel 2", description = "Description 2" }
            };
            _mockChannelService.Setup(s => s.GetChannelsByProjectIdPerUser(projectId, 1)).ReturnsAsync(channels);
            
            var actionResult = await _channelController.GetChannelsByProjectId(projectId);
            actionResult.Should().BeOfType<OkObjectResult>();
            
            var okResult = actionResult as OkObjectResult;
            okResult!.Value.Should().BeOfType<List<Channel>>();
            okResult.Value.Should().BeEquivalentTo(channels);
        }
        
        [Fact]
        public async Task ChannelController_GetChannelsByProjectIdAndUser_ReturnsOk()
        {
            int projectId = 1;
            List<Channel> channels = new List<Channel>
            {
                new Channel() { id = 1, name = "Channel 1", description = "Description 1" },
                new Channel() { id = 2, name = "Channel 2", description = "Description 2" }
            };
            
            _mockChannelService.Setup(s => s.GetChannelsByProjectIdPerUser(projectId, 1)).ReturnsAsync(channels);
            
            var actionResult = await _channelController.GetChannelsByProjectId(projectId);
            actionResult.Should().BeOfType<OkObjectResult>();
            
            var okResult = actionResult as OkObjectResult;
            okResult!.Value.Should().BeOfType<List<Channel>>();
            okResult.Value.Should().BeEquivalentTo(channels);
        }
    }
}