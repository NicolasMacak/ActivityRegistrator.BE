using Microsoft.AspNetCore.Mvc;

namespace ActivityRegistrator.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorizationController : ControllerBase
    {
        public AuthorizationController() { }

        [HttpGet("signout")]
        public Task SignOutAsync()
        {

        }
    }
}
