using Microsoft.AspNetCore.Mvc;
using Thiskord_Back.Services;
using Thiskord_Back.Models.GestionProjet;

namespace Thiskord_Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SprintController : Controller
    {
        private readonly SprintService _sprintService;
        public SprintController(SprintService sprintService) 
        { 
            _sprintService = sprintService;
        }

        [HttpPost("create/sprint")]
        public IActionResult createSprint([FromBody] Sprint req)
        {
            try
            {
                _sprintService.createSprint(req);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        
        [HttpDelete("{id:int}")]
        public IActionResult deleteSprint(int id)
        {
            try
            {
                int res = _sprintService.deleteSprint(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpGet("sprint/{id:int}")]
        public IActionResult getSprint(int id)
        {
            try
            {
                Sprint res = new Sprint();
                res = _sprintService.getSprint(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("sprint")]
        public IActionResult updateSprint([FromBody] Sprint req)
        {
            try
            {
                int res = _sprintService.updateSprint(req);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
