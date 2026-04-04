using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Thiskord_Back.Models.Project;
using Thiskord_Back.Services;
using Thiskord_Back.Tests.Setup;

namespace Thiskord_Back.Tests.UnitTests.Services
{
    public class ProjectServiceTests : IClassFixture<TestDatabaseFixture>
    {
        private readonly ProjectService _projectService;
        TestDatabaseFixture _fixture;
        
        public ProjectServiceTests(TestDatabaseFixture fixture)
        {
            _fixture = fixture;
            var mockDbService = new Mock<IDbConnectionService>();
            mockDbService
                .Setup(db => db.CreateConnection())
                .Returns(() => new SqlConnection(fixture.ConnectionString));
            
            _projectService = new ProjectService(mockDbService.Object, null);
        }

        [Fact]
        public void ProjectService_Create_ReturnsProject()
        {
            Project project = _projectService.Create("ProjectTest", "ProjectTestDescription");
            project.Should().NotBeNull();
            project.name.Should().Be("ProjectTest");
            project.description.Should().Be("ProjectTestDescription");
        }

        [Fact]
        public void ProjectService_DeleteById_ReturnsVoid()
        {
            int projectId = 3;
            _projectService.DeleteById(projectId);
            using var conn = new SqlConnection(_fixture.ConnectionString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT * FROM Project WHERE project_id = @projectId", conn);
            cmd.Parameters.AddWithValue("@projectId", projectId);
            string? result = cmd.ExecuteScalar()?.ToString();
            result.Should().BeNull();
        }

        [Fact]
        public void ProjectService_Update_ReturnsProject()
        {
            Project newProject = new Project
            {
                id = 2,
                name = "ProjectTest",
                description = "ProjectTestDescription",
            };
            var res = _projectService.Update(newProject);
            res.Should().NotBeNull();
            res.Should().BeOfType<Project>();
            res.id.Should().Be(2);
            res.name.Should().Be("ProjectTest");
            res.description.Should().Be("ProjectTestDescription");
        }
        
        [Fact]
        public void ChannelService_GetChannelsByProjectId_ReturnsListChannels()
        {
            List<Project> projects = _projectService.GetAll();
            projects.Should().BeOfType<List<Project>>();
            projects.Count.Should().BeGreaterThan(0);
            projects.Count.Should().Be(3);
        }
    }
}