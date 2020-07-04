using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectManager.Users;
using ProjectManager.Utils;

namespace ProjectManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IdentityProvider _identityProvider;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(IdentityProvider identityProvider, ILogger<AuthenticationController> logger)
        {
            _identityProvider = identityProvider;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult CheckIfAuthenticated()
        {
            if (HttpContext.Session.IsAuthenticated())
            {
                return Ok();
            }

            return Unauthorized();
        }
        
        [HttpPost]
        public IActionResult Authenticate([FromBody] UserRequest loginRequest)
        {
            try
            {
                var user = _identityProvider.Authenticate(loginRequest.Name, loginRequest.Password);
                HttpContext.Session.SetUser(user);
                return Ok();
            }
            catch (ArgumentException e)
            {
                return BadRequest(new SimpleMessageResponse(e.Message));
            }
            catch (AuthenticationException e)
            {
                return Unauthorized(new SimpleMessageResponse(e.Message));
            } 
            catch (Exception e)
            {
                _logger.LogError(e, "Error occured while trying to add user");
            }

            return new StatusCodeResult(500);
        }

        [HttpDelete]
        public IActionResult InvalidateSession()
        {
            HttpContext.Session.Clear();
            return Ok();
        }
    }
}