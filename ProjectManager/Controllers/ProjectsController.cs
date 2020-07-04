using System;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.Projects;
using ProjectManager.Users;
using ProjectManager.Utils;

namespace ProjectManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly ProjectsRegistry _projectsRegistry;
        private readonly UserRegistry _userRegistry;

        public ProjectsController(ProjectsRegistry projectsRegistry, UserRegistry userRegistry)
        {
            _projectsRegistry = projectsRegistry;
            _userRegistry = userRegistry;
        }

        [HttpGet]
        public IActionResult List()
        {
            if (!HttpContext.Session.IsAuthenticated()) return Unauthorized();

            return Ok(_projectsRegistry.GetAll());
        }

        [HttpPost]
        public IActionResult CreateProject([FromBody] CreateProjectRequest createProjectRequest)
        {
            if (!HttpContext.Session.IsAuthenticated()) return Unauthorized();

            var userId = HttpContext.Session.GetCurrentUserId();
            if (userId == null) return Unauthorized();
            try
            {
                var user = _userRegistry.Find((int) userId);
                return Ok(_projectsRegistry.Create(createProjectRequest.Name, createProjectRequest.HourValue, user));
            }
            catch (ArgumentException e)
            {
                return BadRequest(new SimpleMessageResponse(e.Message));
            }
        }

        [HttpPost("{projectId}")]
        public IActionResult Join(int projectId)
        {
            if (!HttpContext.Session.IsAuthenticated()) return Unauthorized();

            var userId = HttpContext.Session.GetCurrentUserId();
            if (userId == null) return Unauthorized();
            try
            {
                var user = _userRegistry.Find((int) userId);
                _projectsRegistry.Join(user, projectId);
            }
            catch (Exception e) when (e is UserNotExistsException || e is ProjectNotExistsException)
            {
                return Forbid();
            }

            return Ok();
        }

        [HttpDelete("{projectId}")]
        public IActionResult Delete(int projectId)
        {
            if (!HttpContext.Session.IsAuthenticated()) return Unauthorized();

            var userId = HttpContext.Session.GetCurrentUserId();
            if (userId == null) return Unauthorized();
            try
            {
                var user = _userRegistry.Find((int) userId);
                _projectsRegistry.Delete(user, projectId);
            }
            catch (Exception e) when (e is ProjectNotExistsException || e is ArgumentException)
            {
                return new ObjectResult(new SimpleMessageResponse(e.Message))
                {    
                    StatusCode = 403
                };
            }

            return Ok();
        }

        [HttpGet("{projectId}/members")]
        public IActionResult ListMembers(int projectId)
        {
            if (!HttpContext.Session.IsAuthenticated()) return Unauthorized();

            try
            {
                return Ok(_projectsRegistry.Find(projectId));
            }
            catch (ProjectNotExistsException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{projectId}/members")]
        public IActionResult RemoveFromProject(int projectId)
        {
            if (!HttpContext.Session.IsAuthenticated()) return Unauthorized();

            var userId = HttpContext.Session.GetCurrentUserId();
            if (userId == null) return Unauthorized();
            try
            {
                var user = _userRegistry.Find((int) userId);
                _projectsRegistry.RemoveFromProject(user, projectId);
                return Ok();
            }
            catch (Exception e) when (e is UserNotExistsException || e is ProjectNotExistsException || e is ArgumentException)
            {
                return Forbid();
            }
        }
    }

    public class CreateProjectRequest
    {
        public string Name { get; set; }
        public float HourValue { get; set; }
    }
}