using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Thiskord_Back.Services;

namespace Thiskord_Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChannelController : ControllerBase
    {
        private ChannelService _channelService;

        public ChannelController(ChannelService channelService)
        {
            _channelService = channelService;
        }

        [HttpPost("channel")]
        
        public IActionResult CreateChannel()
        {
            try
            {
                _channelService.Create("general2", "canal general2");
                return Ok(new { resultat = "success" });

            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }
        [HttpDelete("{id:int}")]
        public IActionResult DeleteChannel(int id)
        {
            try
            {
                _channelService.DeleteById(id);
                return Ok(new { resultat = "success" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }
    }
}
