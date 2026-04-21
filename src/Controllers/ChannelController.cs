using System.Security.Claims;
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
        
        public async Task<IActionResult> CreateChannel([FromBody] ChannelRequest req)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await _channelService.Create(req.name, req.description, req.projectId, userId);
                return Ok(new { resultat = "success" });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteChannel(int id)
        {
            try
            {
                await _channelService.DeleteById(id);
                return Ok(new { resultat = "success" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateChannel([FromBody] ChannelRequest req, int id)
        {
            try
            {
                await _channelService.Update(id, req.name, req.description);
                return Ok(new { resultat = "success" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }
        [HttpGet("project/{projectId:int}")]
        public async Task<IActionResult> GetChannelsByProjectId(int projectId)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var channels = await _channelService.GetChannelsByProjectIdPerUser(projectId, userId);
                return Ok(channels);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }
    }
}
