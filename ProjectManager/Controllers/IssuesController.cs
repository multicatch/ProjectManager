using System;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.Database.Models;
using ProjectManager.Projects;
using ProjectManager.Projects.Issues;
using ProjectManager.Users;
using ProjectManager.Utils;

namespace ProjectManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IssuesController : ControllerBase
    {
        private readonly IssueRegistry _issueRegistry;
        private readonly UserRegistry _userRegistry;

        public IssuesController(IssueRegistry issueRegistry, UserRegistry userRegistry)
        {
            _issueRegistry = issueRegistry;
            _userRegistry = userRegistry;
        }

        [HttpGet("{issueId}")]
        public IActionResult GetIssueDetails(int issueId)
        {
            if (!HttpContext.Session.IsAuthenticated()) return Unauthorized();
            try
            {
                var issues = _issueRegistry.Get(issueId);
                return Ok(_issueRegistry.ConvertToDetails(issues));
            }
            catch (Exception e) when (e is IssueNotFoundException)
            {
                return Forbid();
            }
        }

        [HttpGet("user/{userId}")]
        public IActionResult ListForUser(int userId)
        {
            if (!HttpContext.Session.IsAuthenticated()) return Unauthorized();
            try
            {
                var user = _userRegistry.Find(userId);
                var issues = _issueRegistry.GetAllForUser(user)
                    .ConvertAll(_issueRegistry.ConvertToDetails);
                return Ok(issues);
            }
            catch (Exception e) when (e is UserNotExistsException || e is ProjectNotExistsException || e is ArgumentException)
            {
                return Forbid();
            }
        }

        [HttpGet("project/{projectId}")]
        public IActionResult ListForProject(int projectId)
        {
            if (!HttpContext.Session.IsAuthenticated()) return Unauthorized();

            try
            {
                return Ok(_issueRegistry.GetAllForProject(projectId)
                    .ConvertAll(_issueRegistry.ConvertToDetails));
            }
            catch (Exception e) when (e is UserNotExistsException || e is ProjectNotExistsException || e is ArgumentException)
            {
                return Forbid();
            }
        }

        [HttpPost("project/{projectId}")]
        public IActionResult CreateIssue(int projectId, [FromBody] CreateIssueRequest request)
        {
            if (!HttpContext.Session.IsAuthenticated()) return Unauthorized();
            var userId = HttpContext.Session.GetCurrentUserId();
            if (userId == null) return Unauthorized();
            try
            {
                var user = _userRegistry.Find((int) userId);
                var assignee = _userRegistry.Find(request.Assignee);
                var issue = _issueRegistry.Create(user,
                    projectId,
                    request.Name,
                    request.Description,
                    request.EstimateHours,
                    request.Type,
                    request.Status,
                    assignee,
                    request.Parent
                );
                return Ok(_issueRegistry.ConvertToDetails(issue));
            }
            catch (ArgumentException e)
            {
                return BadRequest(new SimpleMessageResponse(e.Message));
            }
            catch (Exception e) when (e is UserNotExistsException || e is ProjectNotExistsException)
            {
                return Forbid();
            }
        }
    }

    public class CreateIssueRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public float? EstimateHours { get; set; } = null;
        public IssueType Type { get; set; }
        public IssueStatus Status { get; set; }
        public string Assignee { get; set; }
        public int? Parent { get; set; } = null;
    }
}