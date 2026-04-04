using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Thiskord_Back.Models.Channel;
using Thiskord_Back.Services;

namespace Thiskord_Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChannelController : ControllerBase
    {
        private IChannelService _channelService;

        public ChannelController(IChannelService channelService)
        {
            _channelService = channelService;
        }

        [HttpPost("create")]
        
        public IActionResult CreateChannel([FromBody] ChannelRequest req)
        {
            try
            {
                _channelService.Create(req.name, req.description, req.projectId);
                return Ok(new { resultat = "success" });

            }
            catch (Exception ex)
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
        [HttpPut("{id:int}")]
        public IActionResult UpdateChannel([FromBody] ChannelRequest req, int id)
        {
            try
            {
                _channelService.Update(id, req.name, req.description);
                return Ok(new { resultat = "success" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }
        [HttpGet("project/{projectId:int}")]
        public IActionResult GetChannelsByProjectId(int projectId)
        {
            try
            {
                var channels = _channelService.GetChannelsByProjectId(projectId);
                return Ok(channels);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }
    }
}
