using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectManager.Users;
using ProjectManager.Utils;

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

        [HttpGet]
        public IActionResult GetCurrentUserData()
        {
            if (!HttpContext.Session.IsAuthenticated()) return Unauthorized();

            var userId = HttpContext.Session.GetCurrentUserId();
            if (userId == null) return Unauthorized();
            try
            {
                var currentUserData = _userRegistry.Find((int) userId);
                var userInfo = new UserInfoResponse
                {
                    Id = currentUserData.Id,
                    Name = currentUserData.Name
                };
                return Ok(userInfo);
            }
            catch (Exception e) when (e is UserNotExistsException)
            {
                return Forbid();
            }
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

    public class UserInfoResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}