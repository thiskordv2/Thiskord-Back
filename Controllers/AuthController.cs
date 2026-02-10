using Microsoft.AspNetCore.Mvc;
using Thiskord_Back.Models.Auth;
using Thiskord_Back.Services;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Thiskord_Back.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly JsonService _jsonService;

        public AuthController(AuthService authService, JsonService jsonService)
        {
            _authService = authService;
            _jsonService = jsonService;
        }

        [HttpPost("auth")]
        public IActionResult authentification([FromBody] AuthRequest req)
        {
            AuthenticatedUser res = _authService.AuthLogin(req.user_auth, req.password);
            return Ok(res);
        }
    }
}
