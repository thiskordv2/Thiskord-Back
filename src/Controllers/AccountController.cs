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

        [HttpPut("account")]
        public IActionResult patchAccount([FromBody] UpdateAccountRequest req)
        {
            _accountService.patchAccount(req);
            return Ok();
        }

        [HttpPut("account-password")]
        public IActionResult patchAccountPassword([FromBody] UpdatePasswordRequest req)
        {
            return Ok(_accountService.patchAccountPassword(req));
        }
    }
}
