using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Thiskord_Back.Services;
using System.Text.Json;

namespace Thiskord_Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InscriptionController : ControllerBase
    {
        private readonly IInscriptionService _inscriptionService;
        
        public InscriptionController(IInscriptionService inscriptionService, LogService logService)
        {
            _inscriptionService = inscriptionService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] JsonElement req)
        {
            try
            {
                string userName = req.GetProperty("user_name").GetString();
                string userMail = req.GetProperty("user_mail").GetString();
                string password = req.GetProperty("password").GetString();
        
                string userPicture = "defaut";
                if (req.TryGetProperty("user_picture", out JsonElement pictureElement))
                {
                    var picture = pictureElement.GetString();
                    if (!string.IsNullOrEmpty(picture))
                    {
                        userPicture = picture;
                    }
                }

                var createdUser = await _inscriptionService.InscriptionUser(userName, userMail, password, userPicture);
                return Ok(createdUser);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An error occurred while processing the request." + ex.Message });
            }
        }
        

    }
}