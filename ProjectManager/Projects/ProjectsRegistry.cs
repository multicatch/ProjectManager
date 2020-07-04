using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProjectManager.Database;
using ProjectManager.Database.Models;
using ProjectManager.Utils;

namespace ProjectManager.Projects
{
    public class ProjectsRegistry
    {
        private readonly DatabaseContext _databaseContext;

        public ProjectsRegistry(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public List<ProjectDetails> GetAll()
        {
            return _databaseContext.Projects
                .Include(p => p.Members)
                .ToList()
                .ConvertAll(p =>
                {
                    var members = p.Members?.ConvertAll(m => m.User.Name) ?? new List<string>();
                    return new ProjectDetails(p.Id, p.Name, p.HourValue, p.Creator.Name, members);
                });
        }

        public Project Find(int id)
        {
            var project = _databaseContext
                .Projects
                .Include(p => p.Members)
                .AsEnumerable()
                .SingleOrDefault(p => p.Id == id);

            return Validate.NotNullOrElseThrow(project,
                () => new ProjectNotExistsException("Project with Id " + id + " does not exist.")
            );
        }

        public ProjectDetails GetDetails(int id)
        {
            var project = Find(id);
            var members = project.Members?.ConvertAll(m => m.User.Name) ?? new List<string>();
            return new ProjectDetails(project.Id, project.Name, project.HourValue, project.Creator.Name, members);
        }

        public ProjectDetails Create(string name, float hourValue, User creator)
        {
            Validate.NotNullOrBlank(name, "Name cannot be blank.");
            Validate.NotNegative(hourValue, "Hour Value cannot be negative.");
            
            var project = new Project
            {
                Name = name,
                HourValue = hourValue,
                Members = new List<UserProject>(),
                Creator = creator,
                Issues = new List<Issue>()
            };
            _databaseContext.Add(project);
            _databaseContext.SaveChanges();
            Join(creator, project.Id);
            return new ProjectDetails(project.Id, project.Name, project.HourValue, project.Creator.Name, new List<string>
            {
                creator.Name
            });
        }

        public void Join(User user, int projectId)
        {
            var project = Find(projectId);
            var userProject = new UserProject
            {
                ProjectId = projectId,
                Project = project,
                UserId = user.Id,
                User = user
            };
            project.Members ??= new List<UserProject>();
            project.Members.Add(userProject);
            user.Projects ??= new List<UserProject>();
            user.Projects.Add(userProject);
            _databaseContext.Add(userProject);
            _databaseContext.SaveChanges();
        }

        public void RemoveFromProject(User user, int projectId)
        {
            var project = Find(projectId);
            if (user.Id == project.Creator.Id)
            {
                throw new ArgumentException("User is the creator of the project.");
            } 
            project.Members ??= new List<UserProject>();
            var userProject = project.Members.Find(p => p.User == user);
            project.Members = project.Members.FindAll(p => p.User != user);
            user.Projects = user.Projects.FindAll(p => p.Project != project);
            if (userProject != null)
            {
                _databaseContext.Remove(userProject);
            }

            _databaseContext.SaveChanges();
        }

        public void Delete(User currentUser, int projectId)
        {
            var project = Find(projectId);
            if (project.Creator.Id != currentUser.Id)
            {
                throw new ArgumentException("User is not allowed to delete project.");
            }
            project.Members ??= new List<UserProject>();
            project.Members.ForEach(m => _databaseContext.Remove(m));
            project.Issues ??= new List<Issue>();
            project.Issues.ForEach(i => _databaseContext.Remove(i));
            _databaseContext.Remove(project);
            _databaseContext.SaveChanges();
        }
    }

    public class ProjectDetails
    {
        public int Id { get; }
        public string Name { get; }
        public float HourValue { get; }
        
        public string Creator { get; }
        public List<string> Members { get; }
        
        public ProjectDetails(int id, string name, float hourValue, string creator, List<string> members)
        {
            Id = id;
            Name = name;
            HourValue = hourValue;
            Creator = creator;
            Members = members;
        }
    }

    public class ProjectNotExistsException : Exception
    {
        public ProjectNotExistsException(string message) : base(message)
        {
        }
    }
}