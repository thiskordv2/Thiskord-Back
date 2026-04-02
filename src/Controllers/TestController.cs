using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using Thiskord_Back.Services;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Thiskord_Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {

        private readonly IDbConnectionService _dbService;

        private readonly LogService _logService;

        public TestController(IDbConnectionService dbService, LogService logService)
        {
            _dbService = dbService;
            _logService = logService;
        }

        [HttpPost("insert")]
        public IActionResult InsertTest()
        {
            _dbService.Test();
            return Ok(new { message = "Insertion réussie !" });
        }

        [HttpPost("connard")]
        public IActionResult testing()
        {
            return Ok(new { message = "ca fonctionne"});
        }

        public class RequestTest
        {
            public int userId;
            public string message;
        }

        [HttpPost("minotaurd")]
        public IActionResult addLog([FromBody] RequestTest request)
        {
            int userId = request.userId;
            string message = request.message;
            _logService.AddLog(userId, message);
            return Ok(new { message = "on ajouter le log" });
        }




        // GET: api/<TestController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            
            return new string[] { "value1", "value2" };
        }

        // GET api/<TestController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<TestController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<TestController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TestController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
