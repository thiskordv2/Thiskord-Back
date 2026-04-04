using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Thiskord_Back.Controllers;
using Thiskord_Back.Models.Project;
using Thiskord_Back.Services;

namespace Thiskord_Back.Tests.UnitTests.Controllers
{
    public class ProjectControllerTests
    {
        private readonly ProjectController _projectController;
        private readonly Mock<IProjectService> _mockProjectService;

        public ProjectControllerTests()
        {
            _mockProjectService = new Mock<IProjectService>();
            _projectController = new ProjectController(_mockProjectService.Object);
        }

        [Fact]
        public void projectController_CreateProject_ReturnsOk()
        {
            var request = new ProjectRequest() { name = "Test project", description = "This is a test project" };
            var responseproject = new Project() { id = 1, name = request.name, description = request.description }; 
            _mockProjectService.Setup(s => s.Create(request.name, request.description)).Returns(responseproject);
            
            var actualResult = _projectController.CreateProject(request);

            actualResult.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void projectController_DeleteProject_ReturnsOk()
        {
            int id = 1;
            _mockProjectService.Setup(s => s.DeleteById(id));
            var actualResult = _projectController.DeleteProject(id);
            actualResult.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void projectController_UpdateProject_ReturnsOk()
        {
            var project = new Project() { id = 1, name = "Test project", description = "This is a test project" };
            _mockProjectService.Setup(s => s.Update(project)).Returns(project);

            var actionResult = _projectController.UpdateProject(new ProjectRequest() { name = "Test project", description = "This is a test project" }, 1);
            actionResult.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void projectController_GetAllProjects_ReturnsOk()
        {
            List<Project> projects = new List<Project>
            {
                new Project() { id = 1, name = "project 1", description = "Description 1" },
                new Project() { id = 2, name = "project 2", description = "Description 2" }
            };
            _mockProjectService.Setup(s => s.GetAll()).Returns(projects);
            
            var actionResult = _projectController.GetAllProjects();
            actionResult.Should().BeOfType<OkObjectResult>();
            
            var okResult = actionResult as OkObjectResult;
            okResult!.Value.Should().BeOfType<List<Project>>();
            okResult.Value.Should().BeEquivalentTo(projects);
        }
    }
}