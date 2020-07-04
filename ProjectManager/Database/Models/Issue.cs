using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManager.Database.Models
{
    public class Issue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float? EstimateHours { get; set; }
        public IssueType Type { get; set; }
        public IssueStatus Status { get; set; }
        public User Assignee { get; set; }
        public Issue Parent { get; set; }
        public Project Project { get; set; }
        [InverseProperty("Parent")]
        public List<Issue> Children { get; set; }
    }

    public enum IssueType
    {
        Requirement,
        Task
    }

    public enum IssueStatus
    {
        Accepted,
        InProgress,
        Testing,
        Resolved,
        Rejected
    }
}