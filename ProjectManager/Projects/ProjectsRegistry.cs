using System;
using System.Collections.Generic;
using System.Linq;
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
                .ToList()
                .ConvertAll(p =>
                {
                    var members = p.Members?.ConvertAll(m => m.User.Name) ?? new List<string>();
                    return new ProjectDetails(p.Id, p.Name, p.HourValue, members);
                });
        }

        public Project Find(int id)
        {
            var project = _databaseContext.Projects.AsEnumerable().SingleOrDefault(p => p.Id == id);

            return Validate.NotNullOrElseThrow(project,
                () => new ProjectNotExistsException("Project with Id " + id + " does not exist.")
            );
        } 

        public Project Create(string name, float hourValue)
        {
            Validate.NotNullOrBlank(name, "Name cannot be blank.");
            Validate.NotNegative(hourValue, "Hour Value cannot be negative.");
            
            var project = new Project
            {
                Name = name,
                HourValue = hourValue,
                Members = new List<UserProject>(),
                Issues = new List<Issue>()
            };
            _databaseContext.Add(project);
            _databaseContext.SaveChanges();
            return project;
        }

        public void Join(User user, int projectId)
        {
            var project = Find(projectId);
            var userProject = new UserProject
            {
                Project = project,
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
    }

    public class ProjectDetails
    {
        public int Id { get; }
        public string Name { get; }
        public float HourValue { get;  }
        public List<string> Members { get; }
        
        public ProjectDetails(int id, string name, float hourValue, List<string> members)
        {
            Id = id;
            Name = name;
            HourValue = hourValue;
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