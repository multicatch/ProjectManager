using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProjectManager.Database;
using ProjectManager.Database.Models;
using ProjectManager.Utils;

namespace ProjectManager.Projects.Issues
{
    public class IssueRegistry
    {
        private readonly DatabaseContext _databaseContext;
        private readonly ProjectsRegistry _projectsRegistry;

        public IssueRegistry(DatabaseContext databaseContext, ProjectsRegistry projectsRegistry)
        {
            _databaseContext = databaseContext;
            _projectsRegistry = projectsRegistry;
        }

        public Issue Get(int id)
        {
            var issue = _databaseContext
                .Issues
                .Include(i => i.Children)
                .Include(i => i.Project)
                .Include(i => i.Parent)
                .AsEnumerable()
                .SingleOrDefault(i => i.Id == id);

            return Validate.NotNullOrElseThrow(issue,
                () => new IssueNotFoundException("Issue with Id " + id + " does not exist.")
            );
        }

        public IssueDetails GetDetails(int id)
        {
            var issue = Get(id);
            return convertToDetails(issue);
        }

        public List<Issue> GetAllForUser(User user)
        {
            return _databaseContext.Issues
                .Include(i => i.Project)
                .Include(i => i.Assignee)
                .Include(i => i.Parent)
                .Where(i => i.Assignee.Id == user.Id)
                .ToList();
        }

        public List<Issue> GetAllForProject(int projectId)
        {
            return _databaseContext.Issues
                .Include(i => i.Project)
                .Where(i => i.Project.Id == projectId)
                .ToList();
        }
        
        public IssueDetails convertToDetails(Issue i)
        {
            return new IssueDetails(
                i.Id,
                i.Name,
                i.Description,
                i.EstimateHours,
                i.Type,
                i.Status,
                i.Assignee.Name,
                i.Parent?.Id,
                _projectsRegistry.GetDetails(i.Project.Id),
                i.Children.ConvertAll(c => c.Id)
            );
        }

        public Issue Create(User user, int projectId, string name, string description, float? estimateHours,
            IssueType type, IssueStatus status, User assignee, int? parent)
        {
            var project = _projectsRegistry.Find(projectId);
            Validate.NotNullOrElseThrow(project.Members.Find(u => u.User.Id == user.Id), () =>
                throw new ArgumentException("User " + user.Id + " is not a project member."));

            Validate.NotNullOrBlank(name, "Name cannot be blank");
            if (estimateHours != null)
            {
                Validate.NotNegative((float) estimateHours, "Estimate cannot be negative.");
            }
            
            var issue = new Issue
            {
                Name = name,
                Description = description,
                Assignee = assignee,
                EstimateHours = estimateHours,
                Type = type,
                Status = status
            };

            if (parent != null)
            {
                var parentIssue = Get((int) parent);
                issue.Parent = parentIssue;
                parentIssue.Children.Add(issue);
            }
            _databaseContext.Add(issue);
            project.Issues ??= new List<Issue>();
            project.Issues.Add(issue);
            assignee.Issues ??= new List<Issue>();
            assignee.Issues.Add(issue);
            _databaseContext.SaveChanges();

            return issue;
        }
    }

    public class IssueDetails
    {
        public IssueDetails(int id, string name, string description, float? estimateHours, IssueType type,
            IssueStatus status, string assignee, int? parent, ProjectDetails project, List<int> children)
        {
            Id = id;
            Name = name;
            Description = description;
            EstimateHours = estimateHours;
            Type = type;
            Status = status;
            Assignee = assignee;
            Parent = parent;
            Project = project;
            Children = children;
        }

        public int Id { get; }
        public string Name { get; }
        public string Description { get; }
        public float? EstimateHours { get; }
        public IssueType Type { get; }
        public IssueStatus Status { get; }
        public string Assignee { get; }
        public int? Parent { get; }
        public ProjectDetails Project { get; }
        public List<int> Children { get; }
    }

    public class IssueNotFoundException : Exception
    {
        public IssueNotFoundException(string message) : base(message)
        {
        }
    }
}