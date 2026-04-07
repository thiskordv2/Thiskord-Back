using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Thiskord_Back.Services;
using Thiskord_Back.Models;

namespace Thiskord_Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InviteController : ControllerBase
    {
        private readonly IInviteService _inviteService;
        private readonly ILogService _logService;

        public InviteController(IInviteService inviteService, ILogService logService)
        {
            _inviteService = inviteService;
            _logService = logService;
        }

        [HttpPost("{token}")]
        public async Task<IActionResult> AcceptInvite(string token)
        {
            try
            {
                var success = await _inviteService.AcceptInvite(token);
                if (!success) return BadRequest(new { error = "Invalid or expired token" });
                return Ok(new { resultat = "Vous avez rejoint le projet avec succès" });
            }
            catch (Exception ex)
            {
                _logService.CreateLog($"Error in AcceptInvite: {ex.Message}");
                return  Ok(new{ error = ex.Message });
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateInvite([FromBody] CreateInviteRequest request)
        {
            try
            {
                var invite = await _inviteService.CreateInvite(request.projectId, request.creatorId, request.expiresAt);
                return Ok(new { token = invite });
            }
            catch (ArgumentException ex)
            {
                _logService.CreateLog($"Error in CreateInvite: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}

