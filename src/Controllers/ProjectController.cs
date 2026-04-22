using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Thiskord_Back.Models.Project;
using Thiskord_Back.Services;

namespace Thiskord_Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectController : ControllerBase
    {
        private IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpPost("create")]
        public async Task <IActionResult> CreateProject([FromBody] ProjectRequest req)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await _projectService.Create(req.name, req.description, userId);
                return Ok(new { resultat = "success" });

            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }
        [HttpDelete("{id:int}")]
        public IActionResult DeleteProject(int id)
        {
            try
            {
                _projectService.DeleteById(id);
                return Ok(new { resultat = "success" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }
        [HttpPut("{id:int}")]
        public IActionResult UpdateProject([FromBody] ProjectRequest req, int id)
        {
            try
            {
                Project updateProject = new Project
                {
                    id = req.id,
                    name = req.name,
                    description = req.description
                };
                _projectService.Update(updateProject);
                return Ok(new { resultat = "success" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }
        
        [HttpGet("all")]
        public async Task<IActionResult> GetAllProjects()
        {
            try 
            { 
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)); 
                var projects = await _projectService.GetAllProjectsForUser(userId); 
                return Ok(projects);
            }
            catch (Exception ex) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }
    }
}