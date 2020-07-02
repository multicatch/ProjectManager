using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectManager.Users;

namespace ProjectManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserRegistry _userRegistry;
        private readonly ILogger<UserController> _logger;

        public UserController(UserRegistry userRegistry, ILogger<UserController> logger)
        {
            _userRegistry = userRegistry;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] UserRequest createUserRequest)
        {
            try
            {
                _userRegistry.Create(createUserRequest.Name, createUserRequest.Password);
                return Ok();
            }
            catch (ArgumentException e)
            {
                return BadRequest(new SimpleMessageResponse(e.Message));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occured while trying to add user");
            }

            return new StatusCodeResult(500);
        }
    }

    public class UserRequest
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }
}