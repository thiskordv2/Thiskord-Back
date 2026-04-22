using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Thiskord_Back.Models.Account;
using Thiskord_Back.Models.Auth;
using Thiskord_Back.Services;

namespace Thiskord_Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly AccountService _accountService;

        public AccountController(AccountService accountService) 
        {
            _accountService = accountService;
        }

        [HttpGet("account/{id}")]
        public IActionResult getAccount(int id)
        {
            UserAccount res = _accountService.getAccount(id);
            return Ok(res);
        }

        [HttpPost("account")]
        public IActionResult patchAccount([FromBody] UserAccount req)
        {
            _accountService.patchAccount(req);
            return Ok();
        }
        [HttpDelete("account/{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            await _accountService.DeleteAccount(id);
            return NoContent();
        }
        [HttpPost("account-password")]
        public IActionResult patchAccountPassword([FromBody] UserAccount req)
        {
            return Ok(_accountService.patchAccountPassword(req));
        }
    }
}
