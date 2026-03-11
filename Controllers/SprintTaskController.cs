using Microsoft.AspNetCore.Mvc;
using Thiskord_Back.Services;
using Thiskord_Back.Models.GestionProjet;

namespace Thiskord_Back.Controllers
{
    public class SprintTaskController : Controller
    {
        private readonly TaskService _sprintTaskService;
        public SprintTaskController(TaskService sprintTaskService)
        {
            _sprintTaskService = sprintTaskService;
        }

        [HttpGet("sprint/task/{id:int}")]
        public IActionResult getTaskBySprint(int id)
        {
            try
            {
                List<SprintTask> res = _sprintTaskService.getTaskBySprint(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("parent/task/{id:int}")]
        public IActionResult getTaskByParent(int id)
        {
            try
            {
                List<SprintTask> res = _sprintTaskService.getTaskByParentTask(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("task")]
        public IActionResult createTask([FromBody] SprintTask req)
        {
            try
            {
                _sprintTaskService.createTask(req);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("task/{id:int}")]
        public IActionResult deleteTask(int id)
        {
            try
            {
                int res = _sprintTaskService.deleteTask(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("task")]
        public IActionResult updateTask([FromBody] SprintTask req)
        {
            try
            {
                int res = _sprintTaskService.updateTask(req);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
