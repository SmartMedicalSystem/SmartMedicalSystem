using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MEDSYstemITI.Controllers
{
    [ApiController]
    [Route("test")]
    public class TestController : ControllerBase
    {
        [HttpGet("protected")]
        [Authorize(Policy = "Permission:ReadDoctor")]
        public IActionResult Protected() => Ok("ok");

        [HttpGet("anon")]
        [AllowAnonymous]
        public IActionResult Anonymous() => Ok("anon");
    }
}
