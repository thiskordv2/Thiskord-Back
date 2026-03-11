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

        [HttpPost("sprint")]
        public void createSprint([FromBody] Sprint req)
        {

        }

        
        [HttpDelete("{id:int}")]
        public IActionResult deleteSprint(int id)
        {
            int res = _sprintService.deleteSprint(id);
            return Ok();
        }

    }
}
