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
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("auth")]
        public IActionResult authentification([FromBody] AuthRequest req)
        {
            AuthenticatedUser res = _authService.AuthLogin(req.user_auth, req.password);
            if (res == null) return Unauthorized("Identifiants invalides");
            return Ok(res);
        }
    }
}
