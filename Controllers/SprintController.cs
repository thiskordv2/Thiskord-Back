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
            _sprintService.createSprint(req);
            return Ok();
        }

        
        [HttpDelete("{id:int}")]
        public IActionResult deleteSprint(int id)
        {
            int res = _sprintService.deleteSprint(id);
            return Ok();
        }

        [HttpGet("sprint/{id:int}")]
        public IActionResult getSprint(int id)
        {
            return Ok(_sprintService.getSprint(id));
        }

    }
}
