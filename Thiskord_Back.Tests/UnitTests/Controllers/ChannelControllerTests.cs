using FluentAssertions;
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
        }

        [Fact]
        public void ChannelController_CreateChannel_ReturnsOk()
        {
            var request = new ChannelRequest() { name = "Test Channel", description = "This is a test channel", projectId = 1};
            var responseChannel = new Channel() { id = 1, name = request.name, description = request.description }; 
            _mockChannelService.Setup(s => s.Create(request.name, request.description, request.projectId)).Returns(responseChannel);
            
            var actualResult = _channelController.CreateChannel(request);

            actualResult.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void ChannelController_DeleteChannel_ReturnsOk()
        {
            int id = 1;
            _mockChannelService.Setup(s => s.DeleteById(id));
            var actualResult = _channelController.DeleteChannel(id);
            actualResult.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void ChannelController_UpdateChannel_ReturnsOk()
        {
            int id = 1;
            string name = "Test Channel";
            string description = "This is a test channel";
            var channel = new Channel() { id = 1, name = "Test Channel", description = "This is a test channel" };
            _mockChannelService.Setup(s => s.Update(id, name, description)).Returns(channel);

            var actionResult = _channelController.UpdateChannel(new ChannelRequest() { name = name, description = description }, id);
            actionResult.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void ChannelController_GetChannelsByProjectId_ReturnsOk()
        {
            int projectId = 1;
            List<Channel> channels = new List<Channel>
            {
                new Channel() { id = 1, name = "Channel 1", description = "Description 1" },
                new Channel() { id = 2, name = "Channel 2", description = "Description 2" }
            };
            _mockChannelService.Setup(s => s.GetChannelsByProjectId(projectId)).Returns(channels);
            
            var actionResult = _channelController.GetChannelsByProjectId(projectId);
            actionResult.Should().BeOfType<OkObjectResult>();
            
            var okResult = actionResult as OkObjectResult;
            okResult!.Value.Should().BeOfType<List<Channel>>();
            okResult.Value.Should().BeEquivalentTo(channels);
        }
    }
}